using AutoMapper;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.APP.ViewModels.AppliedStudents;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    [Authorize(Roles ="Admin, SuperAdmin")]
    public class AppliedStudentsController : Controller
    {
        private readonly IAppliedStudentManager _appliedStudentManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _host;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IStudentManager _studentManager;

        public AppliedStudentsController(IAppliedStudentManager appliedStudentManager, IMapper mapper, IWebHostEnvironment host, SignInManager<IdentityUser> signInManager, IStudentManager studentManager)
        {
            _appliedStudentManager = appliedStudentManager;
            _mapper = mapper;
            _host = host;
            _signInManager = signInManager;
            _studentManager = studentManager;
        }
        public async Task<IActionResult> Index(string usrtext, string sortRoll, string sortHSCRoll, int page, int pagesize)
        {
            if (TempData["notSaved"] != null)
            {
                ViewBag.notSaved = TempData["notSaved"].ToString();
            }
            if (TempData["saved"] != null)
            {
                ViewBag.saved = TempData["saved"].ToString();
            }
            IQueryable<AppliedStudent> appliedStudentList = _appliedStudentManager.GetIQueryableData();
            ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : " ";
            ViewBag.action = "Index";
            ViewBag.controller = "AppliedStudents";

            switch (sortRoll)
            {
                case "desc":
                    appliedStudentList = appliedStudentList.OrderByDescending(m => m.NUAdmissionRoll);
                    break;
                default:
                    appliedStudentList = appliedStudentList.OrderBy(m => m.NUAdmissionRoll);
                    break;
            }


            ViewBag.data = usrtext;
            

            int pageSize = pagesize <= 0 ? 50 : pagesize;
            if (page <= 0) page = 1;
            if (pageSize == 5001)
            {
                pageSize = appliedStudentList.Count();
            }

            if (!string.IsNullOrEmpty(usrtext))
            {
                usrtext = usrtext.Trim().ToLower();

                appliedStudentList = appliedStudentList.Where(m => m.NUAdmissionRoll.ToString().ToLower().Trim() == usrtext || m.ApplicantName.ToLower().Trim().Contains(usrtext) || m.MobileNo.ToLower().Trim() == usrtext || m.HSCGroup.ToLower().Trim() == usrtext || m.FatherName.ToLower().Trim().Contains(usrtext) || m.MotherName.ToLower().Trim().Contains(usrtext));
                ViewBag.count = appliedStudentList.Count();
                if (pageSize == 5001)
                {
                    pageSize = appliedStudentList.Count();
                }
                return View(await PaginatedList<AppliedStudent>.CreateAsync(appliedStudentList, page, pageSize));
            }
                ViewBag.count = appliedStudentList.Count();
            return View(await PaginatedList<AppliedStudent>.CreateAsync(appliedStudentList, page, pageSize));


        }


        [AllowAnonymous]
        public async Task<IActionResult> Create( int nuRoll, int? studentCat)
        {
            string Action = "Search";
            ViewBag.returnAction = Action;
            if (studentCat == 3)
            {
                ViewBag.studentCategory = 3;
                Action = "MastersSearch";
                ViewBag.studentCategoryName = "Masters(MBA)";
                ViewBag.returnAction = Action;
            }
            else if (studentCat == 2)
            {
                ViewBag.studentCategory = 2;
                Action = "ProfessionalSearch";
                ViewBag.studentCategoryName = "Honors(Prfessional)";
                ViewBag.returnAction = Action;
            }
            else if (studentCat == 4)
            {
                ViewBag.studentCategory = 4;
                Action = "MastersSearchGeneral";
                ViewBag.studentCategoryName = "Masters(General)";
                ViewBag.returnAction = Action;
            }
            else if (studentCat == 5)
            {
                ViewBag.studentCategory = 5;
                Action = "DegreeSearch";
                ViewBag.studentCategoryName = "Degree (Pass Course)";
                ViewBag.returnAction = Action;
            }
            else
            {
                ViewBag.studentCategory = 1;
                Action = "Search";
                ViewBag.studentCategoryName = "General";
                ViewBag.returnAction = Action;
            }

            var existAppliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(nuRoll, (int)studentCat);
            if (existAppliedStudent != null)
            {
                TempData["msg"] = "You are already applied";
                return RedirectToAction(Action, "Students");
            }
            ViewBag.nuRoll = nuRoll;
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Create(AppliedStudentVM vModel, int nuRoll, int? studentCat)
        {
            string Action = "Search";
            ViewBag.returnAction = Action;
            if (studentCat == 3)
            {
                ViewBag.studentCategory = 3;
                Action = "MastersSearch";
                ViewBag.studentCategoryName = "Masters(MBA)";
                ViewBag.returnAction = Action;
            }
            else if (studentCat == 2)
            {
                ViewBag.studentCategory = 2;
                Action = "ProfessionalSearch";
                ViewBag.studentCategoryName = "Honors(Prfessional)";
                ViewBag.returnAction = Action;
            }
            else if (studentCat == 4)
            {
                ViewBag.studentCategory = 4;
                Action = "MastersSearchGeneral";
                ViewBag.studentCategoryName = "Masters(General)";
                ViewBag.returnAction = Action;
            }
            else
            {
                ViewBag.studentCategory = 1;
                Action = "Search";
                ViewBag.studentCategoryName = "General";
                ViewBag.returnAction = Action;
            }

            
            if (nuRoll != vModel.NUAdmissionRoll)
            {
                return RedirectToAction(Action, "Students");
            }
            ViewBag.nuRoll = nuRoll;
            if (ModelState.IsValid)
            {
                var existAppliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(vModel.NUAdmissionRoll, (int)studentCat);
                var isMobileNumberUsed = await _appliedStudentManager.GetByMobileNumber(vModel.MobileNo);
                if (isMobileNumberUsed!=null)
                {
                    ViewBag.msg = "Provided mobile number is already exist.";
                    return View(vModel);
                }
                if (existAppliedStudent!=null)
                {
                    TempData["msg"] = "You are already applied";
                    return RedirectToAction(Action, "Students");
                }

                AppliedStudent aStudent = _mapper.Map<AppliedStudent>(vModel);

                await _appliedStudentManager.AddAsync(aStudent);
                if (!_signInManager.IsSignedIn(User))
                {
                    
                    TempData["msg"] = "Information Updated, Search Again";
                    return RedirectToAction(Action, "Students");
                }
                return RedirectToAction("Index");
            }

            return View();
        }


        [Authorize(Roles ="SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var existingStudent = await _appliedStudentManager.GetByIdAsync(id);
            if (existingStudent ==null)
            {
                ViewBag.msg = "Student Not found";
                return View();
            }
            
            return View(existingStudent);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var existingStudent = await _appliedStudentManager.GetByIdAsync(id);
            var admittedStudent = await _studentManager.GetByAdmissionRollAsync(existingStudent.NUAdmissionRoll);
            if (admittedStudent == null)
            {
                var isDeleted = await _appliedStudentManager.RemoveAsync(existingStudent);
                if (isDeleted == true)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.msg = "not Deleted.";
                return View(existingStudent);
            }
            ViewBag.msg = "This student is already admitted";
            return View(existingStudent);
        }

        [HttpGet]
        public IActionResult GetAppliedStudentList()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAppliedStudentList(IFormFile stuList)
        {
            if (stuList == null)
            {
                ViewBag.msg = "Pleas Select a file";
                return View();
            }
            string fileName = $"{ _host.WebRootPath}\\FIleData\\{ stuList.FileName}";
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                stuList.CopyTo(fileStream);
                fileStream.Flush();
            }
            //List<StInfoTable> students = this.GetStudentsList(file.FileName);
            List<AppliedStudent> students = await this.GetStudentsList(fileName);

            return RedirectToAction("Index");
        }

        private async Task<List<AppliedStudent>> GetStudentsList(string fName)
        {
            int notSaved = 0;
            int saved = 0;
            List<AppliedStudent> students = new List<AppliedStudent>();
            var fileName = fName; // $"{Directory.GetCurrentDirectory()}{@"\wwwroot\FileData\"}" + fName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        var existAppliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(Convert.ToInt32(reader.GetValue(0).ToString()), Convert.ToInt32(reader.GetValue(6).ToString()));
                        if (existAppliedStudent != null)
                        {
                            notSaved++;
                            continue;
                        }
                        else
                        {
                            students.Add(new AppliedStudent()
                            {
                                NUAdmissionRoll = Convert.ToInt32(reader.GetValue(0).ToString()),
                                ApplicantName = reader.GetValue(1).ToString(),
                                FatherName = reader.GetValue(2).ToString(),
                                MotherName = reader.GetValue(3).ToString(),
                                MobileNo = reader.GetValue(4).ToString(),
                                HSCGroup = reader.GetValue(5).ToString(),
                                StudentCategoryId = Convert.ToInt32(reader.GetValue(6).ToString()),
                                AcademicSessionId = Convert.ToInt32(reader.GetValue(7).ToString())
                                //NUAdmissionRoll,ApplicantName,FatherName,MotherName,MobileNo,HSCGroup,StudentCategoryId,AcademicSessionId
                            });
                            saved++;
                        }
                    }
                }
            }
            await _appliedStudentManager.UploadAppliedStudentsAsync(students);
            TempData["notSaved"] = notSaved;
            TempData["saved"] = saved;
            return students;
        }

    }
}
