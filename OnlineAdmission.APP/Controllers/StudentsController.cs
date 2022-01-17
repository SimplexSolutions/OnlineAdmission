using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineAdmission.APP.Utilities.Helper;
using OnlineAdmission.APP.Utilities.NagadSetting;
using OnlineAdmission.APP.Utilities.SMS;
using OnlineAdmission.APP.ViewModels;
using OnlineAdmission.APP.ViewModels.Student;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    public class StudentsController : Controller
    {
        private readonly IStudentManager _studentManager;
        private readonly IWebHostEnvironment _host;
        private readonly IMeritStudentManager _meritStudentManager;
        private readonly IAppliedStudentManager _appliedStudentManager;
        private readonly IMapper _mapper;
        private readonly IPaymentTransactionManager _paymentTransactionManager;
        private readonly IDistrictManager _districtManager;
        private readonly ISubjectManager _subjectManager;
        private readonly ISecurityKey _securityKey;
        private readonly ISMSManager _smsManager;
        private readonly IStudentCategoryManager _studentCategoryManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<StudentsController> _logger;
        private readonly IStudentPaymentTypeManager _studentPaymentTypeManager;
        private readonly IMeritTypeManager _meritTypeManager;
        private readonly IAcademicSessionManager _academicSessionManager;
        private readonly IPaymentTypeManager _paymentTypeManager;

        public StudentsController(IStudentManager studentManager, IWebHostEnvironment host, IMeritStudentManager meritStudentManager, IMapper mapper, IDistrictManager districtManager, 
            ISubjectManager subjectManager, IAppliedStudentManager appliedStudentManager, IPaymentTransactionManager paymentTransactionManager, ISecurityKey securityKey, ISMSManager smsManager, 
            UserManager<IdentityUser> userManager, IStudentCategoryManager studentCategoryManager, ILogger<StudentsController> logger,IStudentPaymentTypeManager studentPaymentTypeManager, 
            IMeritTypeManager meritTypeManager, IAcademicSessionManager academicSessionManager,IPaymentTypeManager paymentTypeManager)
        {
            _studentManager = studentManager;
            _host = host;
            _meritStudentManager = meritStudentManager;
            _mapper = mapper;
            _districtManager = districtManager;
            _subjectManager = subjectManager;
            _appliedStudentManager = appliedStudentManager;
            _paymentTransactionManager = paymentTransactionManager;
            _securityKey = securityKey;
            _smsManager = smsManager;
            _userManager = userManager;
            _studentCategoryManager = studentCategoryManager;
            _logger = logger;
            _studentPaymentTypeManager = studentPaymentTypeManager;
            _meritTypeManager = meritTypeManager;
            _academicSessionManager = academicSessionManager;
            _paymentTypeManager = paymentTypeManager;
        }

        ApplicationAPI _api = new ApplicationAPI();
        NagadAPI _nagadAPI = new NagadAPI();

        // GET: StudentsController
        [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
        public async Task<IActionResult> Index(int sessionId, int studentCategory, string usrtext, int page, int pagesize, int categorySubject, int meritType)
        {
            IQueryable<Student> students = _studentManager.GetStudents();
            var studentCategoryFromSession = HttpContext.Session.GetString("studentCategory");
            
            if (  studentCategory>0)
            {
                if (sessionId>0)
                {
                    students = students.Where(s => s.AcademicSessionId == sessionId);
                }

                students = students.Where(s => s.StudentCategoryId == studentCategory);
                if (categorySubject>0)
                {
                    students = students.Where(s => s.SubjectId == categorySubject);
                }
                if (meritType > 0)
                {
                    var meritStudents = await _meritStudentManager.GetAllAsync();
                    var selectedMeritList = meritStudents.Where(m => m.MeritTypeId == meritType).ToList();

                    students = from s in students
                               join m in selectedMeritList on s.NUAdmissionRoll equals m.NUAdmissionRoll
                               select s;
                }
                ViewBag.category = studentCategory;
                ViewBag.subject = categorySubject;
                ViewBag.meritType = meritType;
                HttpContext.Session.SetString("studentCategory", studentCategory.ToString());
                HttpContext.Session.SetString("categorySubject", categorySubject.ToString());
            }
            else if (studentCategory == 0)
            {
                HttpContext.Session.SetString("studentCategory", studentCategory.ToString());
            }
            else if (!string.IsNullOrEmpty(studentCategoryFromSession))
            {
                ViewBag.category = Convert.ToInt32(studentCategoryFromSession);
                students = students.Where(s => s.StudentCategoryId == Convert.ToInt32(studentCategoryFromSession));
            }
            else
            {
                HttpContext.Session.SetString("studentCategory", "");
            }
            int pageSize = pagesize <= 0 ? 20 : pagesize;
            if (page <= 0) page = 1;

            if (usrtext!=null)
            {
                usrtext = usrtext.Trim();
                students = students.Where(s => s.Name.Contains(usrtext) || s.CollegeRoll.ToString().Contains(usrtext) || s.NUAdmissionRoll.ToString().Contains(usrtext) || s.Subject.SubjectName.Contains(usrtext) || s.StudentMobile.ToString().Contains(usrtext));
            }
            
            
            ViewBag.MeritTypeList = new SelectList(await _meritTypeManager.GetAllAsync(), "Id", "MeritTypeName");
            ViewBag.AcademicSessionList = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "SessionName");

            ViewBag.data = usrtext;
            ViewBag.action = "Index";
            ViewBag.controller = "Students";
            ViewBag.StudentCategoryList = new SelectList(await _studentCategoryManager.GetAllAsync(), "Id", "CategoryName",studentCategory);
            var subList = await _subjectManager.GetAllAsync();
            if (studentCategory>0)
            {
                subList =subList.Where(s => s.StudentCategoryId == studentCategory).ToList();
            }
            ViewBag.SubjectList = new SelectList(subList, "Id", "SubjectName", categorySubject);
            ViewBag.Count = students.Count();
            //return View(AdmittedStudents.Where(s => s.Status==true));
            return View(await PaginatedList<Student>.CreateAsync(students, page, pageSize));
        }


        // GET: StudentsController/Create
        [AllowAnonymous, HttpGet]
        public async Task<ActionResult> Create(StudentDynamicInfoVM model)
        {
            if (model.NuRoll>0)
            {
                MeritStudent existingMeritStudent = await _meritStudentManager.GetMeritStudentAsync(model.NuRoll, model.CategoryId,model.MeritTypeId, model.SessionId);
                var existingAppliedStudent = await _appliedStudentManager.GetAppliedStudentAsync(model.NuRoll, model.CategoryId, model.SessionId);
                var existingSubject = await _subjectManager.GetByCodeAsync(existingMeritStudent.SubjectCode);


                int subjectCode = existingSubject.Code;
                int count = await _studentManager.GetCountAsync(existingSubject.Id)+1;
                string sl = count.ToString("D3");
                ViewBag.studentCategory = model.CategoryId;

                StudentCreateVM student = new StudentCreateVM();
                student.Name = existingAppliedStudent.ApplicantName;
                student.FatherName = existingAppliedStudent.FatherName;
                student.MotherName = existingAppliedStudent.MotherName;
                student.StudentMobile = Convert.ToInt32(existingAppliedStudent.MobileNo);
                student.HSCRoll = existingMeritStudent.HSCRoll;
                student.SubjectId = existingSubject.Id;
                student.Subject = existingSubject;
                student.NuAdmissionRoll = model.NuRoll;
                student.StudentCategoryId = (int)existingMeritStudent.StudentCategoryId;
                student.AcademicSessionId = model.SessionId;

                string year = "";
                var AcademicSession = await _academicSessionManager.GetByIdAsync((int)model.SessionId);
                if (AcademicSession != null)
                {
                    if (model.CategoryId == 1)
                    {
                        year = AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4);
                    }
                    else if (model.CategoryId == 2)
                    {
                        year = AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4);
                    }
                    else if (model.CategoryId == 3)
                    {
                        year = AcademicSession.SessionName.Substring(0, 4);
                    }
                    else if (model.CategoryId == 4)
                    {
                        year = (Convert.ToInt32(AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4)) + 1).ToString();
                    }
                    else if (model.CategoryId == 5)
                    {
                        year = AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4);
                    }
                }

                if (subjectCode < 10)
                {
                    student.CollegeRoll = Convert.ToInt32(year.Substring(year.Length - 2) + "0" + subjectCode + "" + sl);
                }
                else
                {
                    student.CollegeRoll = Convert.ToInt32(year.Substring(year.Length - 2) + "" + subjectCode + "" + sl);
                }

                student.DistrictList = new SelectList(await _districtManager.GetAllAsync(), "Id", "DistrictName").ToList();
                return View(student);
            }
            else
            {
                return RedirectToAction("Search");
            }
        }

        // POST: StudentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Create(StudentCreateVM model, IFormFile photo)
        {
            model.DistrictList = new SelectList(await _districtManager.GetAllAsync(), "Id", "DistrictName").ToList();

            //string msg = "";
            var existingSubject = await _subjectManager.GetByIdAsync(model.SubjectId);
            model.Subject = existingSubject;
            var existStudent = await _studentManager.GetStudentAsync(model.NuAdmissionRoll,(int)model.StudentCategoryId, (int)model.AcademicSessionId);

            if (existStudent == null)
            {
                if (ModelState.IsValid)
                {
                    if (photo != null)
                    {
                        int allowedImgSize = 200000;
                        if (photo.Length > allowedImgSize)
                        {
                            ViewBag.msg = "File size should not more than 200 KB.";
                            return View(model);
                        }

                        string[] supportedExt = { ".jpg", ".jpeg", ".png" };
                        bool validImage = false;
                        string ext = Path.GetExtension(photo.FileName);
                        foreach (var item in supportedExt)
                        {
                            if (ext == item)
                            {
                                validImage = true;
                            }
                        }
                        if (validImage==false)
                        {
                            ViewBag.msg = "Image file is not valid. Upload only supported file(.jpg, .png, .jpeg)";
                            return View(model);
                        }
                        string root = _host.WebRootPath;
                        string folder = "Images/Students/";
                        string attachFile = "p_" + model.HSCRoll.ToString().Trim() + "_" + model.HSCPassingYear.ToString().Trim() + ext;
                        string f = Path.Combine(root, folder, attachFile);
                        using (var stream = new FileStream(f, FileMode.Create))
                        {
                            await photo.CopyToAsync(stream);
                            //string msg = "File has been uploaded successfully";
                        }
                        model.Photo = attachFile;
                    }
                    else
                    {
                        //string msg = "Please upload an image";
                    }


                    string year = "";
                    var AcademicSession = await _academicSessionManager.GetByIdAsync((int)model.AcademicSessionId);
                    if (AcademicSession != null)
                    {
                        if (model.StudentCategoryId == 1)
                        {
                            year = AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4);
                        }
                        else if (model.StudentCategoryId == 2)
                        {
                            year = AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4);
                        }
                        else if (model.StudentCategoryId == 3)
                        {
                            year = AcademicSession.SessionName.Substring(0,4);
                        }
                        else if (model.StudentCategoryId == 4)
                        {
                            year = (Convert.ToInt32(AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4))+1).ToString();
                        }
                        else if(model.StudentCategoryId == 5)
                        {
                            year = AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4);
                        }
                    }

                    int subjectCode = existingSubject.Code;
                    int count = await _studentManager.GetCountAsync(existingSubject.Id)+1;
                    string sl = count.ToString("D3");
                    
                    
                    if (subjectCode<10)
                    {
                        model.CollegeRoll = Convert.ToInt32(year.Substring(year.Length - 2) + "0" + subjectCode + "" + sl);
                    }
                    else
                    {
                        model.CollegeRoll = Convert.ToInt32(year.Substring(year.Length - 2) + "" + subjectCode + "" + sl);
                    }
                    
                    
                    Student newStudent = _mapper.Map<Student>(model);                    
                    newStudent.Status = true;
                    newStudent.Photo = model.Photo;
                    newStudent.CreatedAt = DateTime.Now;
                    newStudent.CreatedBy = HttpContext.Session.GetString("User");
                    
                    await _studentManager.AddAsync(newStudent);


                    //////////////////Code for SMS Sending and Saving
                    bool SentSMS = false;
                    string phoneNum = newStudent.StudentMobile.ToString();
                    string msgText = "Congratulations! "+newStudent.Name+", your admission process is completely done. Your college roll is "+newStudent.CollegeRoll;
                    SentSMS = await ESMS.SendSMS("0"+phoneNum, msgText);
                    SMSModel newSMS = new SMSModel() {
                        MobileList = phoneNum,
                        Text = msgText,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "College",
                        Description = "Admission Success"
                    };
                    
                    if (SentSMS==true)
                    {
                        await _smsManager.AddAsync(newSMS);
                    }

                    return RedirectToAction("StudentApply", new { id = newStudent.Id });
                }
            }


            model.DistrictList = new SelectList(await _districtManager.GetAllAsync(), "Id", "DistrictName").ToList();

            return View(model);

        }
        // GET: StudentsController/Edit/5

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> Edit(int id)
        {
            //HttpContext.Session.SetString("UserId");
            var student = await _studentManager.GetByIdAsync(id);

            var subject = await _subjectManager.GetByIdAsync(student.SubjectId);
            StudentEditVM studentEditVM = _mapper.Map<StudentEditVM>(student);
            studentEditVM.Subject = subject;
            studentEditVM.DistrictList = new SelectList(await _districtManager.GetAllAsync(), "Id", "DistrictName").ToList();
            return View(studentEditVM);
        }

        // POST: StudentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> Edit(int id, StudentEditVM student, IFormFile photo)
        {
            string msg = "";
            if (ModelState.IsValid)
            {
                if (photo != null)
                {
                    int allowedImgSize = 200000;
                    if (photo.Length > allowedImgSize)
                    {
                        ViewBag.msg = "File size should not more than 200 KB.";
                        return View(student);
                    }

                    string[] supportedExt = { ".jpg", ".jpeg", ".png" };
                    bool validImage = false;
                    string ext = Path.GetExtension(photo.FileName);
                    foreach (var item in supportedExt)
                    {
                        if (ext == item)
                        {
                            validImage = true;
                        }
                    }
                    if (validImage == false)
                    {
                        ViewBag.msg = "Image file is not valid. Upload only supported file(.jpg, .png, .jpeg)";
                        return View(student);
                    }

                    string root = _host.WebRootPath;
                    string folder = "Images/Students/";
                    string attachFile = "p_" + student.HSCRoll.ToString().Trim() + "_" + student.HSCPassingYear.ToString().Trim() + ext;
                    string f = Path.Combine(root, folder, attachFile);
                    using (var stream = new FileStream(f, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                        msg = "File has been uploaded successfully";
                    }
                    student.Photo = attachFile;
                }
                Student existStudent = _mapper.Map<Student>(student);
                existStudent.UpdatedAt = DateTime.Now;
                existStudent.UpdatedBy = HttpContext.Session.GetString("UserId");
                await _studentManager.UpdateAsync(existStudent);
                return RedirectToAction("Index");
            }
            ViewBag.msg = msg;
            return View(student);
        }

        // GET: StudentsController/Delete/5
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StudentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
        public async Task<ActionResult> IdCard(int studentCategoryId)
        {
            var totalStudent = await _studentManager.GetAllAsync();
            var student = totalStudent.Where(s=> s.Status == true && s.StudentCategoryId != null);
            //List<MeritStudent> meritStudents = new List<MeritStudent>();
            //foreach (var item in student)
            //{
            //    var mStudent = await _meritStudentManager.GetHonsByAdmissionRollAsync(item.NUAdmissionRoll);
            //    meritStudents.Add(mStudent);
            //}
            if (studentCategoryId > 0)
            {
                student = student.Where(s => s.StudentCategoryId == studentCategoryId).ToList();
                ViewBag.count = student.ToList().Count;
            }
            else
            {
                student = null;
                ViewBag.count = 0;
            }
                            
            return View(student);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Sendsms()
        {
            int category = 1;
            var AdmittedStudents = await _studentManager.GetStudentsByCategoryAsync(category);
            
            StudentIndexVM studentIndexVM = new StudentIndexVM();
             studentIndexVM.Students = AdmittedStudents.Where(s => s.Status == true && s.CollegeRoll== 2130001).ToList();
            

            var StudentCategoryList = new SelectList(studentIndexVM.Students);

           // int count = await _studentManager.GetCountAsync(StudentCategoryList.Items);

            //////////////////Code for SMS Sending and Saving
            bool SentSMS = false;
            //studentIndexVM.ToString(). .Students.GetType().GetProperty().
            // studentIndexVM.Students.ToArray().
           
            foreach (var stu in StudentCategoryList.Items)
            {
                Student newStudent = _mapper.Map<Student>(stu);
                 
                string phoneNum = newStudent.StudentMobile.ToString();
                string msgText = "Thanks for taking admission in Hon’s 1st year at Tejgaon College  session (2020-21)." +
                    "Orientation program will be held on 21st October  2021 at 3.00 pm." +
                    "You are asked to attend the Orientation Program." + Environment.NewLine+
                    "Principal, Tejgaon College Dhaka.";
                SentSMS = await ESMS.SendSMS("0" + phoneNum, msgText);
                SMSModel newSMS = new SMSModel()
                {
                    MobileList = phoneNum,
                    Text = msgText,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "College",
                    Description = "Orientation"
                };

                if (SentSMS == true)
                {
                    await _smsManager.AddAsync(newSMS);
                }
            }
            

            //return View(studentIndexVM);
            //return RedirectToAction("SMS/Index");
            return RedirectToAction("Index","SMS");
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string notification, int studentPaymentTypeId)
        {

            if (studentPaymentTypeId <= 0)
            {
                var stuPayType = await _studentPaymentTypeManager.GetAllAsync();
                studentPaymentTypeId = (from spt in stuPayType
                                        where spt.Status == true
                                        select spt.Id).FirstOrDefault();

                if (studentPaymentTypeId <= 0)
                {
                    studentPaymentTypeId = 1;
                }
            }

            var stuPaymentType = await _studentPaymentTypeManager.GetByIdAsync(studentPaymentTypeId);
            var meritType = await _meritTypeManager.GetByIdAsync((int)stuPaymentType.MeritTypeId);
            var category = await _studentCategoryManager.GetByIdAsync((int)stuPaymentType.StudentCategoryId);
            var paymentType = await _paymentTypeManager.GetByIdAsync((int)stuPaymentType.PaymentTypeId);

            StudentDynamicInfoVM studentDynamicInfoVM = new StudentDynamicInfoVM()
            {
                CategoryId = stuPaymentType.StudentCategoryId,
                SessionId = stuPaymentType.AcademicSessionId,
                MeritTypeId = (int)stuPaymentType.MeritTypeId,
                MeritType = meritType,
                PaymentTypeId = stuPaymentType.PaymentTypeId,
                PaymentTypeShortCode = paymentType.PaymentTypeShortCode,
                CategoryShortCode = category.CategoryShortCode,
                StudentPaymentTypeId = studentPaymentTypeId,
                Remarks=stuPaymentType.Remarks
            };
            //StudentCategory studentCategory = await _studentCategoryManager.GetByIdAsync(stuPaymentType.StudentCategoryId);
            ViewBag.FormTitle = stuPaymentType.StudentCategory.CategoryName + " " + stuPaymentType.PaymentType.PaymentTypeName + " " + (stuPaymentType.AcademicSession.SessionName);
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
            }
            ViewBag.notify = notification;

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.Id);
            }

            return View(studentDynamicInfoVM);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Search(StudentDynamicInfoVM model)
        {
            var stuPaymentType = await _studentPaymentTypeManager.GetByIdAsync(model.StudentPaymentTypeId);            

            ViewBag.FormTitle = stuPaymentType.StudentCategory.CategoryName + " " + stuPaymentType.PaymentType.PaymentTypeName + " " + (stuPaymentType.AcademicSession.SessionName);

            if (model.NuRoll>0)
            {
                if (model.NuRoll>9999999 || model.NuRoll<100000)
                {
                    ViewBag.invalidRoll = "Invalid roll. Please enter a valid roll";
                    return View(model);
                }
                if (model.PaymentTypeId == 1) 
                {
                    //int studentCategory = 5;
                    //bool isPaid = false;
                    //ViewBag.nuRoll = degreePassRoll;
                    //if (degreePassRoll > 0)
                    //{
                    var payment = await _paymentTransactionManager.GetPaymentTransactionAsync(model.NuRoll, model.CategoryId, model.SessionId, model.PaymentTypeId);
                    ViewBag.Remarks = stuPaymentType.Remarks;
                    if (payment != null)
                    {
                        ViewBag.isApplicationPaid = true;
                        //return View();
                        ViewBag.paidNotAdmitted = "Congratulations! Your payment is completed";
                        return View(model);
                    }
                    else
                    {
                        var appliedStudent = await _appliedStudentManager.GetAppliedStudentAsync(model.NuRoll, model.CategoryId, model.SessionId);
                        if (appliedStudent == null)
                        {
                            ViewBag.provideAppliedInformation = "Please submit your basic information.";
                            ViewBag.infoCollection = true;
                            return View(model);
                        }
                        else
                        {
                            model.AppliedStudent = appliedStudent;
                            ViewBag.paymentNotCompleted = "Your payment is not completed";
                            ViewBag.isApplicationPaid = false;
                            return View(model);
                        }
                        
                    }
                    
                }
                else 
                {
                    var admittedStu = await _studentManager.GetStudentAsync(model.NuRoll, model.CategoryId, model.SessionId);
                    if (admittedStu != null)
                    {
                        ViewBag.alreadyAdmittedStudent = admittedStu;
                        return View(model);
                    }
                    var meritStudent = await _meritStudentManager.GetMeritStudentAsync(model.NuRoll, model.CategoryId, model.MeritTypeId, model.SessionId);

                    if (meritStudent == null)
                    {
                        ViewBag.NotSelected = "Sorry! You are not selected for admission.";
                        return View(model);
                    }

                    var appliedStudent = await _appliedStudentManager.GetAppliedStudentAsync(model.NuRoll, model.CategoryId, model.SessionId);
                    if (appliedStudent == null)
                    {
                        ViewBag.provideAppliedInformation = "Please submit your basic information.";
                        ViewBag.Remarks = stuPaymentType.Remarks;
                        ViewBag.infoCollection = true;
                        return View(model);
                    }

                    model.MeritStudent = meritStudent;
                    model.AppliedStudent = appliedStudent;
                    model.Subject= await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);
                    if (meritStudent.PaymentStatus == true)
                    {
                        ViewBag.paidNotAdmitted = "Congratulations! Your payment is completed";
                        StudentPaymentType studentPaymentType = await _studentPaymentTypeManager.GetByIdAsync(model.StudentPaymentTypeId);
                        ViewBag.remarks = studentPaymentType.Remarks;
                        return View(model);
                    }
                    else if (meritStudent.PaymentStatus == false)
                    {
                        ViewBag.paymentNotCompleted = "Your payment is not completed";
                        return View(model);
                    }

                    var subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);

                    SelectedStudentVM selectedStudent = new SelectedStudentVM()
                    {
                        MeritStudent = meritStudent,
                        AppliedStudent = appliedStudent,
                        Subject = subject
                    };
                }
                
            }
            else
            {
                ViewBag.msg = "Please enter a valid roll.";

            }
            return View(model);
        }

        private Exception Exception(string v)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ProfessionalSearch(int professionalRoll, string notification)
        {
            if (TempData["msg"] != null)
            {
                ViewBag.miss = TempData["msg"].ToString();
            }
            ViewBag.Roll = professionalRoll;
            ViewBag.notification = notification;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ProfessionalSearch(int professionalRoll)
        {
            int studentCategory = 2;
            bool applicationIsPaid;
            bool addmissionIsPaid ;
            bool admitted;
            bool selected ;
            ViewBag.nuRoll = professionalRoll;
            if (professionalRoll>0)
            {
                var student = await _studentManager.GetByAdmissionRollAsync(professionalRoll, studentCategory);
                if (student!=null)
                {
                    admitted = true;
                    ViewBag.admitted = admitted;
                    ViewBag.student = student;
                    return View();
                }
                
                var AdmissionPayment = await _paymentTransactionManager.GetAdmissionTrByNuRoll(professionalRoll, studentCategory);

                if (AdmissionPayment == null)
                {
                    var applicationPayment = await _paymentTransactionManager.GetApplicationTransactionByNuRollAsync(professionalRoll, studentCategory);
                    if (applicationPayment!=null)
                    {
                        applicationIsPaid = true;
                        ViewBag.applicationIsPaid = applicationIsPaid;

                        var selectedStudent = await _meritStudentManager.GetProByAdmissionRollAsync(professionalRoll);
                        if (selectedStudent == null)
                        {
                            ViewBag.msg = "You are not eligible";
                            return View();
                        }
                        var appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(professionalRoll,studentCategory);
                        var subject = await _subjectManager.GetByCodeAsync(selectedStudent.SubjectCode);
                        SelectedStudentVM selectedStudentVM = new SelectedStudentVM();
                        selectedStudentVM.AppliedStudent = appliedStudent;
                        selectedStudentVM.MeritStudent = selectedStudent;
                        selectedStudentVM.Subject = subject;

                        ViewBag.allStudentVM = selectedStudentVM;


                        if (selectedStudent!=null)
                        {
                            selected = true;
                            ViewBag.selected = selected;
                            return View();
                        }
                        ViewBag.msg = "You are not selected";
                        return View();
                    }
                    ViewBag.msg = "You didn't apply";
                    return View();
                }
                else 
                {
                    selected = true;
                    addmissionIsPaid = true;
                    ViewBag.addmissionIsPaid = addmissionIsPaid;
                    applicationIsPaid = true;
                }

                ViewBag.selected = selected;
                ViewBag.addmissionIsPaid = addmissionIsPaid;
                ViewBag.applicationIsPaid = applicationIsPaid;
            }
            ViewBag.nuRoll = professionalRoll;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult MastersSearch(int mastersRoll, string notification)
        {
            if (TempData["msg"] != null)
            {
                ViewBag.miss = TempData["msg"].ToString();
                ViewBag.msg = TempData["msg"].ToString();
            }
            if (TempData["infoCollection"] != null)
            {
                ViewBag.infoCollection = TempData["infoCollection"].ToString();
            }
            if (TempData["mastersRoll"] != null)
            {
                ViewBag.Roll = TempData["mastersRoll"].ToString();
            } 
            ViewBag.Roll = mastersRoll;
            ViewBag.notification = notification;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> MastersSearch(int mastersRoll)
        {
            int studentCategory = 3;
            bool applicationIsPaid = false;
            bool addmissionIsPaid = false;
            bool admitted = false;
            bool selected =false ;
            ViewBag.msg = "Your admission successfully completed.";
            ViewBag.admitted = admitted;
            ViewBag.nuRoll = mastersRoll;
            ViewBag.DueAmount = false;

            if (mastersRoll > 0)
            {
                var student = await _studentManager.GetByAdmissionRollAsync(mastersRoll, studentCategory);
                if (student != null)
                {
                    admitted = true;
                    ViewBag.admitted = admitted;
                    ViewBag.student = student;
                    return View();
                }
                var meritStudent = await _meritStudentManager.GetProMBAByAdmissionRollAsync(mastersRoll);
                var appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(mastersRoll, studentCategory);
                
                if (meritStudent==null)
                {
                    ViewBag.msg = "You are not selected";
                    selected = false;
                    ViewBag.selected = selected;
                    return View();
                }                
                
                selected = true;

                var applicationPayment = await _paymentTransactionManager.GetApplicationTransactionByNuRollAsync(mastersRoll, studentCategory);
                if (applicationPayment==null)
                {
                    applicationIsPaid = false;
                    ViewBag.msg = "You are not applied";
                    ViewBag.applicaitonIsPaid = applicationIsPaid;
                    return View();
                }
                applicationIsPaid = true;
                ViewBag.applicationIsPaid = applicationIsPaid;

                if (appliedStudent == null)
                {
                    ViewBag.msg = "Congratulations! You are selected. Now, please submit your basic information.";
                    ViewBag.infoCollection = true;
                    ViewBag.nuRoll = mastersRoll;
                    return View();
                }
                var AdmissionPayment = await _paymentTransactionManager.GetAdmissionTrByNuRoll(mastersRoll, studentCategory);
                if (AdmissionPayment == null)
                {
                    addmissionIsPaid = false;
                    ViewBag.addmissionIsPaid = addmissionIsPaid;
                    ViewBag.msg = "Congratulations! You are selected. Now, please complete your admission fee.";
                    return View();
                }
                
                addmissionIsPaid = true;
                ViewBag.msg = "Your payment is completed";
                var subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);
                SelectedStudentVM selectedStudentVM = new SelectedStudentVM();
                selectedStudentVM.AppliedStudent = appliedStudent;
                selectedStudentVM.MeritStudent = meritStudent;
                selectedStudentVM.Subject = subject;

                ViewBag.meritStudent = meritStudent;
                ViewBag.allStudentVM = selectedStudentVM;                                                    
                ViewBag.addmissionIsPaid = addmissionIsPaid;                    
                ViewBag.AppliedStudent = appliedStudent;
                ViewBag.selected = selected;
                ViewBag.addmissionIsPaid = addmissionIsPaid;
                ViewBag.applicationIsPaid = applicationIsPaid;
                ViewBag.AppliedStudent = appliedStudent;
            }
            else
            {
                ViewBag.selected = selected;
                ViewBag.addmissionIsPaid = addmissionIsPaid;
                ViewBag.applicationIsPaid = applicationIsPaid;
                ViewBag.admitted = admitted;
            }

            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult MastersSearchGeneral(int mastersGenRoll, string notification)
        {
            if (TempData["msg"] != null)
            {
                ViewBag.miss = TempData["msg"].ToString();
            }
            ViewBag.Roll = mastersGenRoll;
            ViewBag.notification = notification;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> MastersSearchGeneral(int mastersGenRoll)
        {
            int studentCategory = 4;
            bool isPaid;
            bool admitted;
            ViewBag.nuRoll = mastersGenRoll;

            if (mastersGenRoll > 0)
            {
                var existAdmittedStudent = await _studentManager.GetByAdmissionRollAsync(mastersGenRoll, studentCategory);
                if (existAdmittedStudent!=null)
                {
                    admitted = true;
                    ViewBag.admitted = admitted;
                    ViewBag.student = existAdmittedStudent;

                    return View();
                }
                else
                {
                    //var appliePayment = await _paymentTransactionManager.GetApplicationTransactionByNuRollAsync(mastersGenRoll,studentCategory);
                    //if (appliePayment==null)
                    //{
                    //    ViewBag.msg = "Sorry! You are not applied";
                    //    return View();
                    //}
                    var existMeritStudent = await _meritStudentManager.GetGenMastersByAdmissionRollAsync(mastersGenRoll);
                    if (existMeritStudent == null)
                    {
                        ViewBag.msg = "Sorry! You are not selected";
                        return View();
                    }
                    else
                    {
                        
                        AppliedStudent existingAppliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(mastersGenRoll, studentCategory);
                        if (existingAppliedStudent == null)
                        {
                            ViewBag.existingAppliedStudent = false;
                            ViewBag.msg = "You are selected, please give some information for admission";
                            return View();
                        }

                        ViewBag.msg = "Congratulations! You are selected"; 
                        ViewBag.NuAdmissionRoll = mastersGenRoll;
                        Subject selectedSubject = await _subjectManager.GetByCodeAsync(existMeritStudent.SubjectCode);


                        SelectedStudentVM selectedStudentVM = new SelectedStudentVM();
                        selectedStudentVM.Subject = selectedSubject;
                        selectedStudentVM.AppliedStudent = existingAppliedStudent;
                        selectedStudentVM.MeritStudent = existMeritStudent;

                        var exisitingAdmissionPayment = await _paymentTransactionManager.GetAdmissionTrByNuRoll(mastersGenRoll, studentCategory);
                        if (exisitingAdmissionPayment!=null)
                        {
                            isPaid = true;
                            ViewBag.isPaid = isPaid;
                            ViewBag.msg = "Your Payment is completed";
                            return View(selectedStudentVM);
                        }
                        else
                        {
                            isPaid = false;
                            ViewBag.isPaid = isPaid;
                            ViewBag.msg = "Your Payment is not completed";
                            return View(selectedStudentVM);
                        }
                    }
                }
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult DegreeSearch(int degreePassRoll, string notification)
        {
            if (TempData["msg"] != null)
            {
                ViewBag.miss = TempData["msg"].ToString();
            }
            ViewBag.Roll = degreePassRoll;
            ViewBag.notification = notification;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> DegreeSearch(int degreePassRoll)
        {
            int studentCategory = 5;
            bool isPaid = false;
            ViewBag.nuRoll = degreePassRoll;
            if (degreePassRoll > 0)
            {
                var payment = await _paymentTransactionManager.GetApplicationTransactionByNuRollAsync(degreePassRoll, studentCategory);
                if (payment != null)
                {
                    isPaid = true;
                    ViewBag.isPaid = isPaid;
                    return View();
                }
                ViewBag.isPaid = isPaid;
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentConfirmation(int NuAdmissionRoll,int CategoryId,int MeritTypeId, int academicSessionId,
          int  PaymentTransactionId,string notification)
        {
            string msg = "";
            if (NuAdmissionRoll > 0)
            {
                //var meritStudent = await _meritStudentManager.GetMeritStudentAsync(model.NuRoll, model.CategoryId, model.MeritTypeId, model.SessionId);

                //if (meritStudent == null)
                //{
                //    ViewBag.NotSelected = "Sorry! You are not selected for admission.";
                //    return View();
                //}

                //var appliedStudent = await _appliedStudentManager.GetAppliedStudentAsync(model.NuRoll, model.CategoryId, model.SessionId);
               // var meritStudent = await _meritStudentManager.GetHonsByAdmissionRollAsync(NuAdmissionRoll);
                var meritStudent = await _meritStudentManager.GetMeritStudentAsync(NuAdmissionRoll, CategoryId,MeritTypeId, academicSessionId);
                var appliedStudent = await _appliedStudentManager.GetAppliedStudentAsync(NuAdmissionRoll, CategoryId, academicSessionId);

                var subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);

                //var stuPaymentType = await _studentPaymentTypeManager.GetByIdAsync(studentPaymentTypeId);
                //var meritType = await _meritTypeManager.GetByIdAsync((int)meritStudent.MeritTypeId);
                //var category = await _studentCategoryManager.GetByIdAsync((int)meritStudent.StudentCategoryId);
                //PaymentTransaction exPT = await _paymentTransactionManager.GetPaymentTransactionById(PaymentTransactionId);
                //var paymentType = await _paymentTypeManager.GetByIdAsync((int)stuPaymentType.PaymentTypeId);

                SelectedStudentVM selectedStudent = new SelectedStudentVM()
                {
                    MeritStudent = meritStudent,
                    AppliedStudent = appliedStudent,
                    Subject = subject,
                    NuRoll = meritStudent.NUAdmissionRoll,
                    CategoryId = (int)meritStudent.StudentCategoryId,
                    SessionId=(int)meritStudent.AcademicSessionId,
                    MeritTypeId=(int)meritStudent.MeritTypeId
                };


                if (selectedStudent != null)
                {
                    var admittedStudent = await _studentManager.GetStudentAsync(NuAdmissionRoll, CategoryId, academicSessionId);
                    if (admittedStudent != null)
                    {
                        msg = "Congratulations! You are already admitted.";
                        ViewBag.admittedStudent = admittedStudent;
                        return View();
                    }
                    else
                    {

                        ViewBag.nuRoll = NuAdmissionRoll;
                        ViewBag.succussNotification = notification;
                        //ViewBag.collegeRoll = collegeRoll;
                        return View(selectedStudent);
                    }
                }
                else
                {
                    msg = "Sorry! Roll Number not found.";
                }
                ViewBag.msg = msg;
            }
            else
            {
                msg = "Please insert a valid roll first.";

            }
            ViewBag.msg = msg;
            
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> CollegePayment(int nuRoll)
        {
            MeritStudent meritStudent = await _meritStudentManager.GetHonsByAdmissionRollAsync(nuRoll);
            Subject subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);
            AppliedStudent appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(nuRoll, (int)meritStudent.StudentCategoryId);
            TransactionInfo transactionInfo = new TransactionInfo();

            double admissinoFee = 0;
            if (meritStudent != null)
            {
                var deductionAmount = meritStudent.DeductedAmaount;
                if (deductionAmount >= 0)
                {
                    admissinoFee = subject.AdmissionFee - deductionAmount;
                }
                else
                {
                    admissinoFee = subject.AdmissionFee;
                }

                transactionInfo.Name = appliedStudent.ApplicantName;
                transactionInfo.NuRoll = meritStudent.NUAdmissionRoll;
                transactionInfo.Amount = admissinoFee;
                transactionInfo.SubjectName = subject.SubjectName;
                transactionInfo.Subject = subject;
                return View(transactionInfo);
            }


            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> CollegePayment(TransactionInfo model)
        {

            if (ModelState.IsValid)
            {

                PaymentTransaction newPayment = new PaymentTransaction();

                newPayment.Amount = model.PaymentTransaction.Amount;
                newPayment.TransactionDate = DateTime.Today;
                newPayment.Balance = 0000;
                newPayment.AccountNo = "College";
                var guid = Guid.NewGuid();
                newPayment.TransactionId = guid.ToString();
                newPayment.ReferenceNo = model.PaymentTransaction.ReferenceNo;
                newPayment.AdmissionFee = model.Amount;
                //newPayment.SubjectId = model.Subject.Id;
                //newPayment.ApplicantName = model.Name;
                await _paymentTransactionManager.AddAsync(newPayment);

                MeritStudent meritStudent = await _meritStudentManager.GetHonsByAdmissionRollAsync(model.PaymentTransaction.ReferenceNo);
                if (meritStudent != null)
                {
                    meritStudent.PaymentStatus = true;
                    PaymentTransaction paymentTransaction = await _paymentTransactionManager.GetAdmissionTrByNuRoll(model.NuRoll, 1);
                    if (paymentTransaction != null)
                    {
                        meritStudent.PaymentTransactionId = paymentTransaction.Id;
                        await _meritStudentManager.UpdateAsync(meritStudent);
                        TempData["msg"] = "Congratulations! Payment Successful.";


                        //////////////////Code for SMS Sending and Saving
                        ///
                        AppliedStudent newStudent = await _appliedStudentManager.GetByAdmissionRollAsync(meritStudent.NUAdmissionRoll, (int)meritStudent.StudentCategoryId);
                        bool SentSMS = false;
                        string phoneNum = newStudent.MobileNo.ToString();
                        string msgText = "Congratulations! " + newStudent.ApplicantName + ", your admission payment is successfully paid";
                        SentSMS = await ESMS.SendSMS("0" + phoneNum, msgText);
                        SMSModel newSMS = new SMSModel()
                        {
                            MobileList = phoneNum,
                            Text = msgText,
                            CreatedAt = DateTime.Now,
                            CreatedBy = "College",
                            Description = "Payment Success"
                        };

                        if (SentSMS == true)
                        {
                            await _smsManager.AddAsync(newSMS);
                        }


                        return RedirectToAction("Search", new { nuAdmissionRoll = model.PaymentTransaction.ReferenceNo });
                    }
                }
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> NagadPayment(StudentDynamicInfoVM model, int? subjectChange)
        {
            if (model.NuRoll<= 0)
            {
               // _logger.LogWarning("NU Roll is Invalid");
                return RedirectToAction("Search");
            }


            string OrderId;
            AppliedStudent appliedStudent = await _appliedStudentManager.GetAppliedStudentAsync(model.NuRoll, model.CategoryId, model.SessionId);
            if (appliedStudent==null)
            {
                _logger.LogWarning("Applied Student Not Found");
                return RedirectToAction("Search");
            }
            
            string paymentshortcode = model.CategoryShortCode + model.PaymentTypeShortCode;
            OrderId = model.NuRoll + "" + paymentshortcode + "" + DateTime.Now.ToString("HHmmss");

            
            
            //}


        #region Initialize API Data Preparation
        ///////////////////////////////////////////////////////// Create JSON Object
        var initializeJSON = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = OrderId,
                datetime = GlobalVariables.RequestDateTime,
                challenge = GlobalVariables.RandomNumber
            };
            // Serialize JSON data to pass through Initialize API
            string initializeJsonData = JsonConvert.SerializeObject(initializeJSON);

            // Encrypt the JSON Data
            string sensitiveData = EncryptWithPublic(initializeJsonData);

            // Generate Signature on JSON Data
            string signatureValue = SignWithMarchentPrivateKey(initializeJsonData);


            // Prepare Final JSON for Initialize API
            var jSON = new
            {
                datetime = GlobalVariables.RequestDateTime,
                sensitiveData = sensitiveData,
                signature = signatureValue
            };
            // Serialize JSON data to pass through Initialize API
            string jSonData = JsonConvert.SerializeObject(jSON);

            #endregion

            ///////////////////////////-/-/-/-//////////////////////////////////////////////////////////////////////////////////////////////

            #region Call Initialize API

            var responseContent = "";
            try
            {
                var httpContent = new StringContent(jSonData, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    //httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "127.0.0.1");
                    httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.1.196");
                    httpClient.DefaultRequestHeaders.Add("X-KM-MC-Id", GlobalVariables.MerchantId);
                    httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-0.2.0");

                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(GlobalVariables.InitializeAPI + GlobalVariables.MerchantId + "/" + OrderId, httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        responseContent = await httpResponse.Content.ReadAsStringAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogWarning(ex.ToString());
               
            }

            Console.WriteLine("Initialize API Response:" + responseContent + "\n");
            #endregion

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            #region Process Initialize API Returned Values & Verify Signature

            dynamic response = JObject.Parse(responseContent);
            string returnedSensitiveData = response.sensitiveData;

            string returnedSignature = response.signature;

            //Decrypt Sensitive Data
            string decryptedSensitiveData = Decrypt(returnedSensitiveData);

            // Initialize API Signature Verification
            var v = Verify(decryptedSensitiveData, returnedSignature, SecurityKey.nagadPublicKey, Encoding.UTF8, HashAlgorithmName.SHA256);
            if (!v)
            {                
                ViewBag.msg = "Signature Verification Failed";
                return View();
            }

            //Process Decrypted Data
            dynamic responsevalue = JObject.Parse(decryptedSensitiveData);
            string challenge = responsevalue.challenge;
            string paymentRefId = responsevalue.paymentReferenceId;
            double totalAmount=0.00;
            double serviceCharge = 0.00;
            double amount = 0.00;
            if (model.PaymentTypeId > 1)
            {
                MeritStudent meritStudent = await _meritStudentManager.GetMeritStudentAsync(model.NuRoll, model.CategoryId, model.MeritTypeId, model.SessionId);
                if (meritStudent == null)
                {
                   // _logger.LogWarning("Merit Student Not Found");
                    return RedirectToAction("Search");
                }
                Subject subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);

                // OrderId = nuRoll.ToString() + "ProAdm" + DateTime.Now.ToString("HHmmss");
                //string paymentshortcode = model.CategoryShortCode + model.PaymentTypeShortCode;
                //OrderId = meritStudent.NUAdmissionRoll + "" + paymentshortcode + "" + DateTime.Now.ToString("HHmmss");
                double paymentForSubjectChange = 0.00;

                //int AA = model.PaymentTypeId;
                if (subjectChange == 1)
                {
                    Student existingStudent = await _studentManager.GetStudentAsync(model.NuRoll, model.CategoryId, model.SessionId);
                    Subject ChangedSubject = await _subjectManager.GetByIdAsync(existingStudent.SubjectId);
                    OrderId = model.NuRoll + "" + paymentshortcode + "" + DateTime.Now.ToString("HHmmss");

                    var payments = await _paymentTransactionManager.GetAllPaymentTrancsactionByNuRoll(model.NuRoll);
                    var paidForAdmission = payments.Where(p => p.PaymentTypeId == 2).Sum(p => p.AdmissionFee);
                    paymentForSubjectChange = ChangedSubject.AdmissionFee - paidForAdmission;
                }
                amount = subject.AdmissionFee - meritStudent.DeductedAmaount;
                if (subjectChange == 1)
                {
                    if (paymentForSubjectChange > 20)
                    {
                        amount = paymentForSubjectChange;
                    }
                }
                //serviceCharge = amount * .0157;
                serviceCharge = amount * .015228;
                totalAmount = Math.Round(amount + serviceCharge);
                serviceCharge = Math.Round((totalAmount * .015), 2);
            }
            else
            {
                var category = await _studentCategoryManager.GetByIdAsync((int)model.CategoryId);
                amount = category.ApplicationFee;
                //serviceCharge = amount * .0157;
                serviceCharge = amount * .015228;
                totalAmount = Math.Round(amount + serviceCharge);
                serviceCharge = Math.Round((totalAmount * .015), 2);
            }
            

            // Create JSON Object
            var paymentJSON = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = OrderId,
                currencyCode = "050",
                amount = totalAmount,
                challenge = challenge
            };

            string paymentJsonData = JsonConvert.SerializeObject(paymentJSON);


            string paymentSensitiveData = EncryptWithPublic(paymentJsonData);

            // Generate Signature on JSON Data
            string paymentSignatureValue = SignWithMarchentPrivateKey(paymentJsonData);

            // Merchant Callback URL
            string merchantCallbackURL = GlobalVariables.merchantCallbackURL;

            //string merchantCallbackURL = "http://sandbox.mynagad.com:10707/merchant-server/web/confirm"; //merchant Callback URL - as you want
            //string merchantCallbackURL = "http://115.127.26.3:4356/api/PaymentTransactions"; //merchant Callback URL - as you want
            //string merchantCallbackURL = "http://115.127.26.3:80/api/PaymentTransactions"; //merchant Callback URL - as you want
            //string merchantCallbackURL = "https://localhost:44356/api/PaymentTransactions"; //merchant Callback URL - as you want
           
            //dynamic additionalMerchantInfo;
            var additionalMerchantInfo = new
            {
                ServiceCharge = serviceCharge,
                AdmissionFee = amount,
                StudentName = appliedStudent.ApplicantName,
                MobileNo = appliedStudent.MobileNo,
                NuAdmissionRoll = model.NuRoll,
                StudentCategory= model.CategoryId,
                PaymentType = model.PaymentTypeId,
                subjectChange = subjectChange,
                meritType = model.MeritTypeId,
                sessionId = model.SessionId
            };

            var paymentFinalJSON = new
            {
                sensitiveData = paymentSensitiveData,
                signature = paymentSignatureValue,
                merchantCallbackURL = merchantCallbackURL,
                additionalMerchantInfo = additionalMerchantInfo

            };

            // Serialize JSON data to pass through Initialize API
            string finalJSONData = JsonConvert.SerializeObject(paymentFinalJSON);

            #endregion
            ///////////////////////////////////////

            #region Call Checkout API
            var br_ResponseContent = "";
            try
            {
                var br_httpContent = new StringContent(finalJSONData, Encoding.UTF8, "application/json");

                using (var br_httpClient = new HttpClient())
                {
                    //br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "115.127.26.3");
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.1.196");
                    //br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "127.0.0.1");
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-MC-Id", GlobalVariables.MerchantId);
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-0.2.0");
                    // Do the actual request and await the response
                    var httpResponse = await br_httpClient.PostAsync(GlobalVariables.CheckOutAPI + paymentRefId, br_httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {                        
                        br_ResponseContent = await httpResponse.Content.ReadAsStringAsync();                      
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            Console.WriteLine("Checkout API Response:" + br_ResponseContent + "\n"); //This is area to show the view
            

            #endregion
            /////////////////////////////////////////////////

            #region Process Checkout API Response
            dynamic co_Response = JObject.Parse(br_ResponseContent);
            string site = co_Response.callBackUrl;
            if (co_Response.status == "Success")
            {
               return Redirect(site);
            }

            else
            {
                
                return View();
            }
                
            #endregion

        }
        
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> NagadPaymentPro(int nuRoll, int studentCategory, string mobileNum, string studentName, int paymentType, string meritType)
        {
            if (nuRoll <= 0)
            {
                TempData["msg"] = "Roll Number is not valid";
                return RedirectToAction("ProfessionalSearch");
            }

            string OrderId="";

            MeritStudent meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(nuRoll, studentCategory, meritType);
            if (meritStudent == null)
            {
                _logger.LogWarning("Merit Student Not Found");
                return RedirectToAction("ProfessionalSearch");
            }
            Subject subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);
            if (subject==null)
            {
                _logger.LogWarning("Subject Not Found");
                return RedirectToAction("ProfessionalSearch");
            }
            if (paymentType == 1)
            {
                OrderId = nuRoll.ToString() + "ProApp" + DateTime.Now.ToString("HHmmss");
            }
            else if (paymentType == 2)
            {
                OrderId = nuRoll.ToString() + "ProAdm" + DateTime.Now.ToString("HHmmss");
                AppliedStudent appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(nuRoll, studentCategory);
                studentName = appliedStudent.ApplicantName;
                mobileNum = appliedStudent.MobileNo;
            }
            else
            {
                _logger.LogError("Student Category is not matched");
                return RedirectToAction("ProfessionalSearch");
            }
            
            #region Initialize API Data Preparation
            ///////////////////////////////////////////////////////// Create JSON Object
            var initializeJSON = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = OrderId,
                datetime = GlobalVariables.RequestDateTime,
                challenge = GlobalVariables.RandomNumber
            };
            // Serialize JSON data to pass through Initialize API
            string initializeJsonData = JsonConvert.SerializeObject(initializeJSON);

            // Encrypt the JSON Data
            string sensitiveData = EncryptWithPublic(initializeJsonData);

            // Generate Signature on JSON Data
            string signatureValue = SignWithMarchentPrivateKey(initializeJsonData);


            // Prepare Final JSON for Initialize API
            var jSON = new
            {
                datetime = GlobalVariables.RequestDateTime,
                sensitiveData = sensitiveData,
                signature = signatureValue
            };
            // Serialize JSON data to pass through Initialize API
            string jSonData = JsonConvert.SerializeObject(jSON);

            #endregion

            ///////////////////////////-/-/-/-//////////////////////////////////////////////////////////////////////////////////////////////

            #region Call Initialize API

            var responseContent = "";
            try
            {
                var httpContent = new StringContent(jSonData, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    //httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "127.0.0.1");
                    httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.1.196");
                    httpClient.DefaultRequestHeaders.Add("X-KM-MC-Id", GlobalVariables.MerchantId);
                    httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-0.2.0");
                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(GlobalVariables.InitializeAPI + GlobalVariables.MerchantId + "/" + OrderId, httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        responseContent = await httpResponse.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }




            Console.WriteLine("Initialize API Response:" + responseContent + "\n");
            #endregion

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            #region Process Initialize API Returned Values & Verify Signature

            dynamic response = JObject.Parse(responseContent);
            string returnedSensitiveData = response.sensitiveData;

            string returnedSignature = response.signature;

            //Decrypt Sensitive Data
            string decryptedSensitiveData = Decrypt(returnedSensitiveData);

            // Initialize API Signature Verification
            var v = Verify(decryptedSensitiveData, returnedSignature, SecurityKey.nagadPublicKey, Encoding.UTF8, HashAlgorithmName.SHA256);
            if (!v)
            {

                ViewBag.msg = "Signature Verification Failed";
                return View();
            }

            //Process Decrypted Data
            dynamic responsevalue = JObject.Parse(decryptedSensitiveData);
            string challenge = responsevalue.challenge;
            string paymentRefId = responsevalue.paymentReferenceId;
            double amount = subject.AdmissionFee - meritStudent.DeductedAmaount;
            double serviceCharge = amount * .0157;
            double totalAmount = Math.Round((amount + serviceCharge), 2);


            // Create JSON Object
            var paymentJSON = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = OrderId,
                currencyCode = "050",
                amount = totalAmount,
                challenge = challenge
            };

            string paymentJsonData = JsonConvert.SerializeObject(paymentJSON);


            string paymentSensitiveData = EncryptWithPublic(paymentJsonData);

            // Generate Signature on JSON Data
            string paymentSignatureValue = SignWithMarchentPrivateKey(paymentJsonData);

            // Merchant Callback URL
            string merchantCallbackURL = GlobalVariables.merchantCallbackURL;

            var additionalMerchantInfo = new
            {
                ServiceCharge = serviceCharge,
                StudentName = studentName,
                MobileNo = mobileNum,
                NuAdmissionRoll = nuRoll,
                AdmissionFee = amount,
                StudentCategory = studentCategory,
                PaymentType = paymentType
            };

            var paymentFinalJSON = new
            {
                sensitiveData = paymentSensitiveData,
                signature = paymentSignatureValue,
                merchantCallbackURL = merchantCallbackURL,
                additionalMerchantInfo = additionalMerchantInfo

            };

            // Serialize JSON data to pass through Initialize API
            string finalJSONData = JsonConvert.SerializeObject(paymentFinalJSON);

            #endregion
            ///////////////////////////////////////

            #region Call Checkout API
            var br_ResponseContent = "";
            try
            {
                var br_httpContent = new StringContent(finalJSONData, Encoding.UTF8, "application/json");

                using (var br_httpClient = new HttpClient())
                {
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.1.196");
                    //br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "127.0.0.1");
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-MC-Id", GlobalVariables.MerchantId);
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-0.2.0");
                    // Do the actual request and await the response
                    var httpResponse = await br_httpClient.PostAsync(GlobalVariables.CheckOutAPI + paymentRefId, br_httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        br_ResponseContent = await httpResponse.Content.ReadAsStringAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Checkout API Response:" + br_ResponseContent + "\n"); //This is area to show the view


            #endregion
            /////////////////////////////////////////////////

            #region Process Checkout API Response
            dynamic co_Response = JObject.Parse(br_ResponseContent);
            string site = co_Response.callBackUrl;
            if (co_Response.status == "Success")
            {
                return Redirect(site);
            }


            else
            {

                return View();
            }

            #endregion

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> NagadPaymentMasters(int nuRoll, int studentCategory, string mobileNum, string studentName, int paymentType, string meritType)
        {
            if (nuRoll <= 0)
            {
                TempData["msg"] = "Roll Number is not valid";
                return RedirectToAction("MastersSearch");
            }

            string OrderId = "";
            MeritStudent meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(nuRoll,studentCategory,meritType);
            Subject subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);
            if (studentCategory == 3)
            {
                if (paymentType==1)
                {
                    if (mobileNum == null || studentName == null)
                    {
                        TempData["miss"] = "Mobile Number and Name is mendatory";
                        return RedirectToAction("MastersSearch", "Students");
                    }
                }
                
                OrderId = nuRoll.ToString() + "" + "promba" + "" + DateTime.Now.ToString("HHmmss");
                if (paymentType==2)
                {
                    OrderId = nuRoll.ToString() + "" + "MBAAdm" + "" + DateTime.Now.ToString("HHmmss");
                    AppliedStudent appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(nuRoll, studentCategory);
                    if (appliedStudent==null)
                    {
                        TempData["msg"] = "Please submit your basic information.";
                        TempData["infoCollection"] = true;
                        TempData["mastersRoll"] = nuRoll;
                        return RedirectToAction("MastersSearch", "Students");
                    }
                    studentName = appliedStudent.ApplicantName;
                    mobileNum = appliedStudent.MobileNo;
                    
                }
            }
            else
            {
                return RedirectToAction("MastersSearch", "Students");
            }

            #region Initialize API Data Preparation
            ///////////////////////////////////////////////////////// Create JSON Object
            var initializeJSON = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = OrderId,
                datetime = GlobalVariables.RequestDateTime,
                challenge = GlobalVariables.RandomNumber
            };
            // Serialize JSON data to pass through Initialize API
            string initializeJsonData = JsonConvert.SerializeObject(initializeJSON);

            // Encrypt the JSON Data
            string sensitiveData = EncryptWithPublic(initializeJsonData);

            // Generate Signature on JSON Data
            string signatureValue = SignWithMarchentPrivateKey(initializeJsonData);


            // Prepare Final JSON for Initialize API
            var jSON = new
            {
                datetime = GlobalVariables.RequestDateTime,
                sensitiveData = sensitiveData,
                signature = signatureValue
            };
            // Serialize JSON data to pass through Initialize API
            string jSonData = JsonConvert.SerializeObject(jSON);

            #endregion

            ///////////////////////////-/-/-/-//////////////////////////////////////////////////////////////////////////////////////////////

            #region Call Initialize API

            var responseContent = "";
            try
            {
                var httpContent = new StringContent(jSonData, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    //httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "127.0.0.1");
                    httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.1.196");
                    httpClient.DefaultRequestHeaders.Add("X-KM-MC-Id", GlobalVariables.MerchantId);
                    httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-0.2.0");
                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(GlobalVariables.InitializeAPI + GlobalVariables.MerchantId + "/" + OrderId, httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        responseContent = await httpResponse.Content.ReadAsStringAsync();

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }




            Console.WriteLine("Initialize API Response:" + responseContent + "\n");
            #endregion

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            #region Process Initialize API Returned Values & Verify Signature

            dynamic response = JObject.Parse(responseContent);
            string returnedSensitiveData = response.sensitiveData;

            string returnedSignature = response.signature;

            //Decrypt Sensitive Data
            string decryptedSensitiveData = Decrypt(returnedSensitiveData);

            // Initialize API Signature Verification
            var v = Verify(decryptedSensitiveData, returnedSignature, SecurityKey.nagadPublicKey, Encoding.UTF8, HashAlgorithmName.SHA256);
            if (!v)
            {

                ViewBag.msg = "Signature Verification Failed";
                return View();
            }

            //Process Decrypted Data
            dynamic responsevalue = JObject.Parse(decryptedSensitiveData);
            string challenge = responsevalue.challenge;
            string paymentRefId = responsevalue.paymentReferenceId;
            //double amount = 300;
            double amount = subject.AdmissionFee - meritStudent.DeductedAmaount;
            double serviceCharge = (amount * .015);
            double totalAmount = amount + serviceCharge;

            // Create JSON Object
            var paymentJSON = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = OrderId,
                currencyCode = "050",
                amount = totalAmount,
                challenge = challenge
            };

            string paymentJsonData = JsonConvert.SerializeObject(paymentJSON);


            string paymentSensitiveData = EncryptWithPublic(paymentJsonData);

            // Generate Signature on JSON Data
            string paymentSignatureValue = SignWithMarchentPrivateKey(paymentJsonData);

            // Merchant Callback URL
            string merchantCallbackURL = GlobalVariables.merchantCallbackURL;

            var additionalMerchantInfo = new
            {
                ServiceCharge = serviceCharge,
                StudentName = studentName,
                MobileNo = mobileNum,
                NuAdmissionRoll = nuRoll,
                AdmissionFee = 0,
                StudentCategory = studentCategory,
                PaymentType = paymentType
            };

            var paymentFinalJSON = new
            {
                sensitiveData = paymentSensitiveData,
                signature = paymentSignatureValue,
                merchantCallbackURL = merchantCallbackURL,
                additionalMerchantInfo = additionalMerchantInfo

            };

            // Serialize JSON data to pass through Initialize API
            string finalJSONData = JsonConvert.SerializeObject(paymentFinalJSON);

            #endregion
            ///////////////////////////////////////

            #region Call Checkout API
            var br_ResponseContent = "";
            try
            {
                var br_httpContent = new StringContent(finalJSONData, Encoding.UTF8, "application/json");

                using (var br_httpClient = new HttpClient())
                {
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.1.196");
                    //br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "127.0.0.1");
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-MC-Id", GlobalVariables.MerchantId);
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-0.2.0");
                    // Do the actual request and await the response
                    var httpResponse = await br_httpClient.PostAsync(GlobalVariables.CheckOutAPI + paymentRefId, br_httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        br_ResponseContent = await httpResponse.Content.ReadAsStringAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Checkout API Response:" + br_ResponseContent + "\n"); //This is area to show the view


            #endregion
            /////////////////////////////////////////////////

            #region Process Checkout API Response
            dynamic co_Response = JObject.Parse(br_ResponseContent);
            string site = co_Response.callBackUrl;
            if (co_Response.status == "Success")
            {
                return Redirect(site);
            }


            else
            {

                return View();
            }

            #endregion

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> NagadPaymentMastersGeneral(int nuRoll, int studentCategory, string mobileNum, string studentName, int paymentType, string meritType)
        {
            if (nuRoll <= 0)
            {
                TempData["msg"] = "Roll Number is not valid";
                return RedirectToAction("MastersSearchGeneral");
            }

            string OrderId = "";
            MeritStudent meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(nuRoll, studentCategory, meritType);
            Subject subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);

            if (studentCategory == 4)
            {
                OrderId = nuRoll.ToString() + "" + "GenMas" + "" + DateTime.Now.ToString("HHmmss");
                if (paymentType == 2)
                {
                    OrderId = nuRoll.ToString() + "" + "MGnAd" + "" + DateTime.Now.ToString("HHmmss");
                    AppliedStudent appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(nuRoll,studentCategory);
                    if (appliedStudent != null)
                    {
                        studentName = appliedStudent.ApplicantName;
                        mobileNum = appliedStudent.MobileNo;
                    }
                }
            }
            else
            {
                return RedirectToAction("MastersSearchGeneral", "Students");
            }

            #region Initialize API Data Preparation
            ///////////////////////////////////////////////////////// Create JSON Object
            var initializeJSON = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = OrderId,
                datetime = GlobalVariables.RequestDateTime,
                challenge = GlobalVariables.RandomNumber
            };
            // Serialize JSON data to pass through Initialize API
            string initializeJsonData = JsonConvert.SerializeObject(initializeJSON);

            // Encrypt the JSON Data
            string sensitiveData = EncryptWithPublic(initializeJsonData);

            // Generate Signature on JSON Data
            string signatureValue = SignWithMarchentPrivateKey(initializeJsonData);


            // Prepare Final JSON for Initialize API
            var jSON = new
            {
                datetime = GlobalVariables.RequestDateTime,
                sensitiveData = sensitiveData,
                signature = signatureValue
            };
            // Serialize JSON data to pass through Initialize API
            string jSonData = JsonConvert.SerializeObject(jSON);

            #endregion

            ///////////////////////////-/-/-/-//////////////////////////////////////////////////////////////////////////////////////////////

            #region Call Initialize API

            var responseContent = "";
            try
            {
                var httpContent = new StringContent(jSonData, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    //httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "127.0.0.1");
                    httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.1.196");
                    httpClient.DefaultRequestHeaders.Add("X-KM-MC-Id", GlobalVariables.MerchantId);
                    httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-0.2.0");
                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(GlobalVariables.InitializeAPI + GlobalVariables.MerchantId + "/" + OrderId, httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        responseContent = await httpResponse.Content.ReadAsStringAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }

            Console.WriteLine("Initialize API Response:" + responseContent + "\n");
            #endregion

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            #region Process Initialize API Returned Values & Verify Signature

            dynamic response = JObject.Parse(responseContent);
            string returnedSensitiveData = response.sensitiveData;

            string returnedSignature = response.signature;

            //Decrypt Sensitive Data
            string decryptedSensitiveData = Decrypt(returnedSensitiveData);

            // Initialize API Signature Verification
            var v = Verify(decryptedSensitiveData, returnedSignature, SecurityKey.nagadPublicKey, Encoding.UTF8, HashAlgorithmName.SHA256);
            if (!v)
            {
                ViewBag.msg = "Signature Verification Failed";
                return View();
            }

            //Process Decrypted Data
            dynamic responsevalue = JObject.Parse(decryptedSensitiveData);
            string challenge = responsevalue.challenge;
            string paymentRefId = responsevalue.paymentReferenceId;

            double amount = subject.AdmissionFee - meritStudent.DeductedAmaount;
            double serviceCharge = amount * .0157;
            double totalAmount = Math.Round((amount + serviceCharge), 2);

            //double amount = 300;
            //double serviceCharge = 5.00;// (amount * .015);
            //double totalAmount = amount + serviceCharge;

            // Create JSON Object
            var paymentJSON = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = OrderId,
                currencyCode = "050",
                amount = totalAmount,
                challenge = challenge
            };

            string paymentJsonData = JsonConvert.SerializeObject(paymentJSON);


            string paymentSensitiveData = EncryptWithPublic(paymentJsonData);

            // Generate Signature on JSON Data
            string paymentSignatureValue = SignWithMarchentPrivateKey(paymentJsonData);

            // Merchant Callback URL
            string merchantCallbackURL = GlobalVariables.merchantCallbackURL;

            var additionalMerchantInfo = new
            {
                ServiceCharge = serviceCharge,
                StudentName = studentName,
                MobileNo = mobileNum,
                NuAdmissionRoll = nuRoll,
                AdmissionFee = subject.AdmissionFee,
                StudentCategory = studentCategory,
                PaymentType = paymentType
            };

            var paymentFinalJSON = new
            {
                sensitiveData = paymentSensitiveData,
                signature = paymentSignatureValue,
                merchantCallbackURL = merchantCallbackURL,
                additionalMerchantInfo = additionalMerchantInfo

            };

            // Serialize JSON data to pass through Initialize API
            string finalJSONData = JsonConvert.SerializeObject(paymentFinalJSON);

            #endregion
            ///////////////////////////////////////

            #region Call Checkout API
            var br_ResponseContent = "";
            try
            {
                var br_httpContent = new StringContent(finalJSONData, Encoding.UTF8, "application/json");

                using (var br_httpClient = new HttpClient())
                {
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.1.196");
                    //br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "127.0.0.1");
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-MC-Id", GlobalVariables.MerchantId);
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-0.2.0");
                    // Do the actual request and await the response
                    var httpResponse = await br_httpClient.PostAsync(GlobalVariables.CheckOutAPI + paymentRefId, br_httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        br_ResponseContent = await httpResponse.Content.ReadAsStringAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Checkout API Response:" + br_ResponseContent + "\n"); //This is area to show the view


            #endregion
            /////////////////////////////////////////////////

            #region Process Checkout API Response
            dynamic co_Response = JObject.Parse(br_ResponseContent);
            string site = co_Response.callBackUrl;
            if (co_Response.status == "Success")
            {
                return Redirect(site);
            }


            else
            {

                return View();
            }

            #endregion

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> NagadPaymentDegreePass(int nuRoll, int studentCategory, string mobileNum, string studentName, int paymentType)
        {
            if (nuRoll <= 0)
            {
                TempData["msg"] = "Roll Number is not valid";
                return RedirectToAction("DegreeSearch");
            }

            string OrderId = "";

            if (studentCategory == 5)
            {
                if (mobileNum == null || studentName == null)
                {
                    TempData["miss"] = "Mobile Number and Name is mendatory";
                    return RedirectToAction("DegreeSearch", "Students");
                }
                OrderId = nuRoll.ToString() + "" + "DegPApp" + "" + DateTime.Now.ToString("HHmmss");
                if (paymentType == 2)
                {
                    OrderId = nuRoll.ToString() + "" + "DegPAdm" + "" + DateTime.Now.ToString("HHmmss");
                    AppliedStudent appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(nuRoll, studentCategory);
                    studentName = appliedStudent.ApplicantName;
                    mobileNum = appliedStudent.MobileNo;
                }
            }
            else
            {
                return RedirectToAction("DegreeSearch", "Students");
            }

            #region Initialize API Data Preparation
            ///////////////////////////////////////////////////////// Create JSON Object
            var initializeJSON = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = OrderId,
                datetime = GlobalVariables.RequestDateTime,
                challenge = GlobalVariables.RandomNumber
            };
            // Serialize JSON data to pass through Initialize API
            string initializeJsonData = JsonConvert.SerializeObject(initializeJSON);

            // Encrypt the JSON Data
            string sensitiveData = EncryptWithPublic(initializeJsonData);

            // Generate Signature on JSON Data
            string signatureValue = SignWithMarchentPrivateKey(initializeJsonData);


            // Prepare Final JSON for Initialize API
            var jSON = new
            {
                datetime = GlobalVariables.RequestDateTime,
                sensitiveData = sensitiveData,
                signature = signatureValue
            };
            // Serialize JSON data to pass through Initialize API
            string jSonData = JsonConvert.SerializeObject(jSON);

            #endregion

            ///////////////////////////-/-/-/-//////////////////////////////////////////////////////////////////////////////////////////////

            #region Call Initialize API

            var responseContent = "";
            try
            {
                var httpContent = new StringContent(jSonData, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    //httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "127.0.0.1");
                    httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.1.196");
                    httpClient.DefaultRequestHeaders.Add("X-KM-MC-Id", GlobalVariables.MerchantId);
                    httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-0.2.0");
                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(GlobalVariables.InitializeAPI + GlobalVariables.MerchantId + "/" + OrderId, httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        responseContent = await httpResponse.Content.ReadAsStringAsync();

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }




            Console.WriteLine("Initialize API Response:" + responseContent + "\n");
            #endregion

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            #region Process Initialize API Returned Values & Verify Signature

            dynamic response = JObject.Parse(responseContent);
            string returnedSensitiveData = response.sensitiveData;

            string returnedSignature = response.signature;

            //Decrypt Sensitive Data
            string decryptedSensitiveData = Decrypt(returnedSensitiveData);

            // Initialize API Signature Verification
            var v = Verify(decryptedSensitiveData, returnedSignature, SecurityKey.nagadPublicKey, Encoding.UTF8, HashAlgorithmName.SHA256);
            if (!v)
            {

                ViewBag.msg = "Signature Verification Failed";
                return View();
            }

            //Process Decrypted Data
            dynamic responsevalue = JObject.Parse(decryptedSensitiveData);
            string challenge = responsevalue.challenge;
            string paymentRefId = responsevalue.paymentReferenceId;
            double amount = 250;

            double serviceCharge = 5.00;// (amount * .015);
            double totalAmount = amount + serviceCharge;

            // Create JSON Object
            var paymentJSON = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = OrderId,
                currencyCode = "050",
                amount = totalAmount,
                challenge = challenge
            };

            string paymentJsonData = JsonConvert.SerializeObject(paymentJSON);


            string paymentSensitiveData = EncryptWithPublic(paymentJsonData);

            // Generate Signature on JSON Data
            string paymentSignatureValue = SignWithMarchentPrivateKey(paymentJsonData);

            // Merchant Callback URL
            string merchantCallbackURL = GlobalVariables.merchantCallbackURL;

            var additionalMerchantInfo = new
            {
                ServiceCharge = serviceCharge,
                StudentName = studentName,
                MobileNo = mobileNum,
                NuAdmissionRoll = nuRoll,
                AdmissionFee = 0,
                StudentCategory = studentCategory,
                PaymentType = paymentType
            };

            var paymentFinalJSON = new
            {
                sensitiveData = paymentSensitiveData,
                signature = paymentSignatureValue,
                merchantCallbackURL = merchantCallbackURL,
                additionalMerchantInfo = additionalMerchantInfo

            };

            // Serialize JSON data to pass through Initialize API
            string finalJSONData = JsonConvert.SerializeObject(paymentFinalJSON);

            #endregion
            ///////////////////////////////////////

            #region Call Checkout API
            var br_ResponseContent = "";
            try
            {
                var br_httpContent = new StringContent(finalJSONData, Encoding.UTF8, "application/json");

                using (var br_httpClient = new HttpClient())
                {
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.1.196");
                    //br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "127.0.0.1");
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-MC-Id", GlobalVariables.MerchantId);
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-0.2.0");
                    // Do the actual request and await the response
                    var httpResponse = await br_httpClient.PostAsync(GlobalVariables.CheckOutAPI + paymentRefId, br_httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        br_ResponseContent = await httpResponse.Content.ReadAsStringAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Checkout API Response:" + br_ResponseContent + "\n"); //This is area to show the view


            #endregion
            /////////////////////////////////////////////////

            #region Process Checkout API Response
            dynamic co_Response = JObject.Parse(br_ResponseContent);
            string site = co_Response.callBackUrl;
            if (co_Response.status == "Success")
            {
                return Redirect(site);
            }


            else
            {

                return View();
            }

            #endregion

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> NagadPayment(TransactionInfo model)
        {
            
            if (ModelState.IsValid)
            {
                
                MeritStudent meritStudent = await _meritStudentManager.GetHonsByAdmissionRollAsync(model.PaymentTransaction.ReferenceNo);

                HttpClient client = _nagadAPI.Initial();

                var postTask = client.PostAsJsonAsync<TransactionInfo>("remote-payment-gateway-1.0/api/dfs/check-out/initialize/"+model.merchantId+"/", model);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["msg"] = "Congratulations! Payment Successful.";
                    return RedirectToAction("Search", new { nuAdmissionRoll = model.PaymentTransaction.ReferenceNo });
                }

                //PaymentTransaction newPayment = new PaymentTransaction();

                //newPayment.Amount = model.PaymentTransaction.Amount; 
                //newPayment.TransactionDate = DateTime.Today;
                //newPayment.Balance = 0000;
                //newPayment.AccountNo = model.PaymentTransaction.AccountNo;           
                //var guid = Guid.NewGuid();
                //newPayment.TransactionId = guid.ToString();
                //newPayment.ReferenceNo = model.PaymentTransaction.ReferenceNo;

                //await _paymentTransactionManager.AddAsync(newPayment);


                //MeritStudent meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(model.PaymentTransaction.ReferenceNo);
                //if (meritStudent !=null)
                //{
                //    meritStudent.PaymentStatus = true;
                //    PaymentTransaction paymentTransaction = await _paymentTransactionManager.GetTransactionByNuRollAsync(model.NuRoll);
                //    if (paymentTransaction != null)
                //    {
                //        meritStudent.PaymentTransactionId = paymentTransaction.Id;
                //        await _meritStudentManager.UpdateAsync(meritStudent);
                //        TempData["msg"] = "Congratulations! Payment Successful.";
                //        return RedirectToAction("Search", new {nuAdmissionRoll = model.PaymentTransaction.ReferenceNo });
                //    }
                //}
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult StudentApply(int? id)
        {
            string msg = "";
            if (id != null || id <= 0)
            {
                msg = "Congratulations! You have submitted admission form successfully.";

            }
            if (TempData["msg"] != null)
            {
                msg = TempData["msg"].ToString();
            }
            ViewBag.msg = msg;

            return View();
        }

        // GET: StudentsController/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentManager.GetByIdAsync((int)id);
            if (student == null)
            {
                return NotFound();
            }
            var sub = await _subjectManager.GetByIdAsync(student.SubjectId);
            
            student.Subject = sub;
            student.PresentDistrict = await _districtManager.GetByIdAsync(student.PresentDistrictId);
            student.PermanentDistrict = await _districtManager.GetByIdAsync(student.PermanentDistrictId);
            student.MailingDistrict = await _districtManager.GetByIdAsync(student.MailingDistrictId);
            student.StudentCategory = await _studentCategoryManager.GetByIdAsync((int)student.StudentCategoryId);
            student.AcademicSession = await _academicSessionManager.GetByIdAsync((int)student.AcademicSessionId);
            MeritStudent meritStudent = await _meritStudentManager.GetHonsByAdmissionRollAsync(student.NUAdmissionRoll);

            

            StudentDetailsVM stuDetails = new StudentDetailsVM();
            stuDetails.Student = student;
            stuDetails.MeritStudent = meritStudent;
            if (student.StudentType == 2)
            {
                Student previouseStudent = await _studentManager.GetByCollegeRollAsync((int)student.PreviousCollegeRoll);
                Subject previousSubject = await _subjectManager.GetByIdAsync(previouseStudent.SubjectId);
                stuDetails.PreviousSubject = previousSubject;
            }
            
            return View(stuDetails);
        }

        
        public async Task<IActionResult> SubjectChange()
        {
            SubjectChangedVM subjectChangedVM = new SubjectChangedVM();
            
            subjectChangedVM.StudentList = new SelectList((from stu in await _studentManager.GetAllAsync()
                                                           where stu.Status == true && (stu.PreviousCollegeRoll == null || stu.PreviousCollegeRoll == 0)
                                                      select new
                                                      {
                                                          Id = stu.Id,
                                                          Name = stu.CollegeRoll +" - "+ stu.Name +" [" + stu.Subject.SubjectName + "] "+ " ( " + stu.NUAdmissionRoll + " ) "
                                                      }), "Id", "Name").ToList();
            var subList = await _subjectManager.GetAllAsync();

            subjectChangedVM.SubjectList = new SelectList((from sub in subList.OrderBy(s => s.Code)
                                                      select new
                                                      {
                                                          Id = sub.Id,
                                                          Name = sub.Code +" - "+ sub.SubjectName
                                                      }), "Id", "Name").ToList();

            return View(subjectChangedVM);
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SubjectChange(SubjectChangedVM model)
        {

            SubjectChangedVM sChangedVM = new SubjectChangedVM();

            sChangedVM.StudentList = new SelectList((from stu in await _studentManager.GetAllAsync()
                                                     where stu.Status == true && (stu.PreviousCollegeRoll == null || stu.PreviousCollegeRoll == 0)
                                                     select new
                                                     {
                                                         Id = stu.Id,
                                                         Name = stu.CollegeRoll + " - " + stu.Name + " [" + stu.Subject.SubjectName + "] " + " ( " + stu.NUAdmissionRoll + " ) "
                                                     }), "Id", "Name").ToList();
            var subList = await _subjectManager.GetAllAsync();

            sChangedVM.SubjectList = new SelectList((from sub in subList.OrderBy(s => s.Code)
                                                     select new
                                                     {
                                                         Id = sub.Id,
                                                         Name = sub.Code + " - " + sub.SubjectName
                                                     }), "Id", "Name").ToList();

            if (model.SubjectId<=0 || model.StudentId<=0)
            {
                return View(sChangedVM);
            }
            
            
            

            if (ModelState.IsValid)
            {
                Student previousStudent = await _studentManager.GetByIdAsync(model.StudentId);
                Subject newSubject = await _subjectManager.GetByIdAsync(model.SubjectId);

                StudentCreateVM studentCreateVM = _mapper.Map<StudentCreateVM>(previousStudent);
                Student NewStudent = _mapper.Map<Student>(studentCreateVM);
                //Creating New College Roll
                int newCollgeRoll = await CreateCollgeRoll(model.SubjectId, (int)previousStudent.AcademicSessionId, (int)previousStudent.StudentCategoryId);
                var isCollegeRollAssign = await _studentManager.GetByCollegeRollAsync(newCollgeRoll);
                if (isCollegeRollAssign != null)
                {
                    ViewBag.msg = "Subject Change cann't be possible due to duplicate roll, Please Contact your technical support";
                    return View(sChangedVM);
                }

                //Update Previous Student
                previousStudent.UpdatedAt = DateTime.Now;
                previousStudent.UpdatedBy = HttpContext.Session.GetString("UserId");
                previousStudent.Status = false;
                previousStudent.StudentType = 1;
                string msgText="";
                double restAmount = newSubject.AdmissionFee - previousStudent.Subject.AdmissionFee;
                if (previousStudent.Subject.AdmissionFee == newSubject.AdmissionFee)
                {
                    previousStudent.Status = false;
                    msgText = NewStudent.Name + ", Your subject changed ("+previousStudent.Subject.SubjectName+" to "+newSubject.SubjectName+") is completed";
                }
                else
                {
                    if (restAmount>0)
                    {
                        ViewBag.msg = "Subject Change will pending due to admission fee missmatch for both subject";
                        msgText = NewStudent.Name + ", Your subject changed is incompleted due to rest payment. Please pay full payment to complete your admission";
                        NewStudent.Status = false;
                    }
                    else
                    {
                        
                        msgText = NewStudent.Name + ", Your subject changed is completed. You can collect "+restAmount+"Tk. from admission office.";
                        NewStudent.Status = true;
                    }

                    
                }
                await _studentManager.UpdateAsync(previousStudent);

                //Create New Student
                NewStudent.CollegeRoll = newCollgeRoll;
                NewStudent.CreatedAt = DateTime.Now;
                NewStudent.CreatedBy = HttpContext.Session.GetString("UserId");
                NewStudent.UpdatedAt = new DateTime();
                NewStudent.UpdatedBy = "";

                var existingStudent = await _studentManager.GetByIdAsync(model.StudentId);
                NewStudent.PreviousCollegeRoll = existingStudent.CollegeRoll;
                NewStudent.SubjectId = model.SubjectId;
                NewStudent.Subject = newSubject;
                NewStudent.StudentType = 2;

                await _studentManager.AddAsync(NewStudent);
                bool SentSMS = false;
                string phoneNum = NewStudent.StudentMobile.ToString();

                SentSMS = await ESMS.SendSMS("0" + phoneNum, msgText);
                SMSModel newSMS = new SMSModel()
                {
                    MobileList = phoneNum,
                    Text = msgText,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "System",
                    Description = "Subject Changed"
                };

                if (SentSMS == true)
                {
                    await _smsManager.AddAsync(newSMS);
                }
                return RedirectToAction("Index");
            }

            ViewBag.msg = "Some informations are missiong, Please Try again later";
            return View(sChangedVM);
        }


        [AllowAnonymous]
        public async Task<JsonResult> SubjectListByStudentId(int id)
        {
            var subject = await _subjectManager.GetByStudentIdAsyc(id);
            List <Subject> subList = await _subjectManager.GetAllAsync();
            var subs = from s in subList.OrderBy(m => m.Code)
                       where s.Id != subject.Id
                       select s;
            return Json(subs);
        }

        [AllowAnonymous]
        public async Task<JsonResult> MeritTypeByStudentCategory(int sessionId, int categoryId)
        {
            var meritTypeList = await _meritTypeManager.GetAllAsync();
            return Json("");
        }
        //Nagad Addition Code here=======================================

        #region Helper Functions

        #region EncryptWithPublicKey
        public static string EncryptWithPublic(string baseText)
        {
            try
            {
                //  System.Diagnostics.Debug.WriteLine("merchantId: " + jsonPlainData.merchantId + "-" + "orderId: " + jsonPlainData.orderId + "-" + "dateTime: " + jsonPlainData.dateTime + "-" + "challenge: " + jsonPlainData.challenge);
                var rng = new Random();
                RSA cipher = myfun(0);
                var plaintext = baseText;
                byte[] data = Encoding.UTF8.GetBytes(plaintext);

                byte[] cipherText = cipher.Encrypt(data, RSAEncryptionPadding.Pkcs1);
                var encryptedData = Convert.ToBase64String(cipherText);
                return encryptedData;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region SignWithMarchentPrivateKey
        public static string SignWithMarchentPrivateKey(string baseText)
        {
            try
            {
                var rsa = myfun(1);
                byte[] dataBytes = Encoding.UTF8.GetBytes(baseText);
                var signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(signatureBytes);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region Decrypt
        public static string Decrypt(string plainText)
        {
            var rsa = myfun(1);
            if (rsa == null)
            {
                throw new Exception("_privateKeyRsaProvider is null");
            }
            string decryptedData = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(plainText), RSAEncryptionPadding.Pkcs1));
            
            return decryptedData;
        }
        #endregion

        #region RSA Encryption
        private static RSA myfun(int num)
        {
            try
            {
                if (num == 1)
                {
                    var privateKeyBytes = Convert.FromBase64String(SecurityKey.marchentPrivateKey);
                    int myarray;
                    var rsa = RSA.Create();

                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out myarray);
                    return rsa;
                }
                if (num == 0)
                {
                    var publicKeyBytes = Convert.FromBase64String(SecurityKey.nagadPublicKey);
                    int myarray;
                    var rsa = RSA.Create();

                    rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out myarray);
                    return rsa;
                }
            }
            catch (CryptographicException e)
            {

                Console.WriteLine(e.Message);
            }

            return null;
        }
        #endregion

        #region Signature Verify
        public static bool Verify(string data, string sign, string publicKey, Encoding encoding, HashAlgorithmName hashAlgorithmName)
        {
            byte[] dataBytes = encoding.GetBytes(data);
            byte[] signBytes = Convert.FromBase64String(sign);
            RSA rsa = CreateRsaProviderFromPublicKey(publicKey);
            var verify = rsa.VerifyData(dataBytes, signBytes, hashAlgorithmName, RSASignaturePadding.Pkcs1);
            return verify;
        }

        private static RSA CreateRsaProviderFromPublicKey(string publicKeyString)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            var x509Key = Convert.FromBase64String(publicKeyString);

            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (MemoryStream mem = new MemoryStream(x509Key))
            {
                using (BinaryReader binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    seq = binr.ReadBytes(15);       //read the Sequence OID
                    if (!CompareBytearrays(seq, seqOid))    //make sure Sequence for OID is correct
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    bt = binr.ReadByte();
                    if (bt != 0x00)     //expect null byte next
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                        lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte(); //advance 2 bytes
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte();    //skip this null byte
                        modsize -= 1;   //reduce modulus buffer size by 1
                    }

                    byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                    if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                        return null;
                    int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                    byte[] exponent = binr.ReadBytes(expbytes);

                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                    var rsa = System.Security.Cryptography.RSA.Create();
                    RSAParameters rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);

                    return rsa;
                }

            }
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        public async Task<int> CreateCollgeRoll(int subId, int sessionId, int categoryId)
        {
            Subject newSubject = await _subjectManager.GetByIdAsync(subId);
            int studentCount = await _studentManager.GetCountAsync(subId) + 1;
            string year = "";
            var AcademicSession = await _academicSessionManager.GetByIdAsync(sessionId);
            if (AcademicSession != null)
            {
                if (categoryId == 1)
                {
                    year = AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4);
                }
                else if (categoryId == 2)
                {
                    year = AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4);
                }
                else if (categoryId == 3)
                {
                    year = AcademicSession.SessionName.Substring(0, 4);
                }
                else if (categoryId == 4)
                {
                    year = (Convert.ToInt32(AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4)) + 1).ToString();
                }
                else if (categoryId == 5)
                {
                    year = AcademicSession.SessionName.Substring(AcademicSession.SessionName.Length - 4);
                }
            }


            int colRoll = Convert.ToInt32(
                year.Substring(DateTime.Today.ToString("yyyy").Length - 2) +
                newSubject.Code.ToString("D2") + studentCount.ToString("D3"));
            return colRoll;
        }
        #endregion

        #endregion

        //public async Task<IActionResult> PaymentReceipt()
        //{
        //    var allStudents = await _studentManager.GetAllAsync();
        //    ViewData["StudentId"] = new SelectList((from s in allStudents
        //                                            select new
        //                                                 {
        //                                                     Id = s.Id,
        //                                                     FullName = s.Name + "(" + s.CollegeRoll + ")"
        //                                                 }), "Id", "FullName", null);


        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> PaymentReceipt(int id)
        //{
        //    var existStudent = await _studentManager.GetByIdAsync(id);
        //    var paymentTransactions = await _paymentTransactionManager.GetAllAsync();
        //    var selectedTransactions = paymentTransactions.Where(t => t.ReferenceNo == existStudent.NUAdmissionRoll).ToList();
        //    var subject = await _subjectManager.GetByIdAsync(existStudent.SubjectId);
        //    PaymentReceiptVM paymentReceiptVM = new PaymentReceiptVM();
        //    paymentReceiptVM.PaymentTransactions = selectedTransactions;
        //    paymentReceiptVM.Student = existStudent;
        //    paymentReceiptVM.Subject = subject;
        //    return View(paymentReceiptVM);
        //}


    }
}
