using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineAdmission.APP.Utilities.Helper;
using OnlineAdmission.APP.Utilities.NagadSetting;
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
    [Authorize]
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

        public StudentsController(IStudentManager studentManager, IWebHostEnvironment host, IMeritStudentManager meritStudentManager, IMapper mapper, IDistrictManager districtManager, ISubjectManager subjectManager, IAppliedStudentManager appliedStudentManager, IPaymentTransactionManager paymentTransactionManager, ISecurityKey securityKey)
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
        }

        ApplicationAPI _api = new ApplicationAPI();
        NagadAPI _nagadAPI = new NagadAPI();

        // GET: StudentsController
        public async Task<IActionResult> Index()
        {
            var AdmittedStudents = await _studentManager.GetAllAsync();
            return View(AdmittedStudents);

            //Code for API Consuming
            //List<Student> students = new List<Student>();
            //HttpClient client = _api.Initial();
            //HttpResponseMessage res = await client.GetAsync("api/student");
            //if (res.IsSuccessStatusCode)
            //{
            //    var result = res.Content.ReadAsStringAsync().Result;
            //    students = JsonConvert.DeserializeObject<List<Student>>(result);
            //}
            //return View(students);

            
        }

        // GET: StudentsController/Create
        [AllowAnonymous]
        public async Task<ActionResult> Create(int nuAdmissionRoll)
        {
            if (nuAdmissionRoll>0)
            {

                var existingMeritStudent = await _meritStudentManager.GetByAdmissionRollAsync(nuAdmissionRoll);
                var existingAppliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(nuAdmissionRoll);
                var existingSubject = await _subjectManager.GetByCodeAsync(existingMeritStudent.SubjectCode);

                string year = DateTime.Today.ToString("yyyy");
                int subjectCode = existingSubject.Code;
                int count = await _studentManager.GetCountAsync(existingSubject.Id);
                string sl = "";
                if (count < 100)
                {
                    if (count == 0)
                    {
                        sl = "001";
                    }
                    else if (count < 10)
                    {
                        sl = "00" + count.ToString();
                    }
                    else if (count < 100 && count > 9)
                    {
                        sl = "0" + count.ToString();
                    }
                }
                else
                {
                    sl = count.ToString();
                }


                //student.CollegeRoll = Convert.ToInt32(year.Substring(year.Length - 2) + "" + subjectCode + "" + sl);

                StudentCreateVM student = new StudentCreateVM();
                student.Name = existingAppliedStudent.ApplicantName;
                student.FatherName = existingAppliedStudent.FatherName;
                student.MotherName = existingAppliedStudent.MotherName;
                student.StudentMobile = Convert.ToInt32(existingAppliedStudent.MobileNo);
                student.HSCRoll = existingMeritStudent.HSCRoll;
                student.SubjectId = existingSubject.Id;
                student.Subject = existingSubject;
                student.CollegeRoll = Convert.ToInt32(year.Substring(year.Length - 2) + "" + subjectCode + "" + sl);

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
        public async Task<ActionResult> Create(StudentCreateVM student, IFormFile photo)
        {


            student.DistrictList = new SelectList(await _districtManager.GetAllAsync(), "Id", "DistrictName").ToList();



            string msg = "";
            var existingSubject = await _subjectManager.GetByIdAsync(student.SubjectId);
            student.Subject = existingSubject;
            var existStudent = await _studentManager.GetStudentByHSCRollAsync(student.HSCRoll);
            if (existStudent != null)
            {
                msg = "HSC Roll is already Exist";
                ViewBag.msg = msg;
                return View(student);
            }
            existStudent = await _studentManager.GetStudentBySSCRollAsync(student.SSCRoll);
            if (existStudent != null)
            {
                msg = "SSC Roll is already Exist";
                ViewBag.msg = msg;
                return View(student);
            }

            if (existStudent == null)
            {
                if (ModelState.IsValid)
                {
                    if (photo != null)
                    {
                        string ext = Path.GetExtension(photo.FileName);
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
                    else
                    {
                        msg = "Please upload an image";
                    }
                    if (student.CollegeRoll == 0)
                    {
                        string year = DateTime.Today.ToString("yyyy");
                        int subjectCode = existingSubject.Code;
                        int count = await _studentManager.GetCountAsync(existingSubject.Id);
                        string sl = "";
                        if (count < 100)
                        {
                            if (count == 0)
                            {
                                sl = "001";
                            }
                            else if (count < 10)
                            {
                                sl = "00" + count.ToString();
                            }
                            else if (count < 100 && count > 9)
                            {
                                sl = "0" + count.ToString();
                            }
                        }
                        else
                        {
                            sl = count.ToString();
                        }


                        student.CollegeRoll = Convert.ToInt32(year.Substring(year.Length - 2) + "" + subjectCode + "" + sl);
                    }
                    
                    
                    Student newStudent = _mapper.Map<Student>(student);
                    newStudent.Photo = student.Photo;
                    newStudent.CreatedAt = DateTime.Now;
                    newStudent.CreatedBy = "Online User"; /*HttpContext.Session.GetString("User")*/
                    
                    await _studentManager.AddAsync(newStudent);
                    return RedirectToAction("StudentApply", new { id = newStudent.Id });
                }
            }


            student.DistrictList = new SelectList(await _districtManager.GetAllAsync(), "Id", "DistrictName").ToList();

            return View(student);

        }
        // GET: StudentsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
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
        public async Task<ActionResult> Edit(int id, StudentEditVM student, IFormFile photo)
        {
            string msg = "";
            if (ModelState.IsValid)
            {
                if (photo != null)
                {
                    string ext = Path.GetExtension(photo.FileName);
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
                await _studentManager.UpdateAsync(existStudent);
            }
            ViewBag.msg = msg;
            return View(student);
        }

        // GET: StudentsController/Delete/5
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


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Search(string notification)
        {

            if (TempData["msg"]!=null)
            {
                ViewBag.msg = TempData["msg"].ToString();
            }
            ViewBag.notify = notification;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Search(int NuAdmissionRoll)
        {
            string msg = "";
            string nuRoll = NuAdmissionRoll.ToString();
            if (NuAdmissionRoll>0)
            {
                var meritStudent =await _meritStudentManager.GetByAdmissionRollAsync(NuAdmissionRoll);
                var appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(NuAdmissionRoll);
                
                if (meritStudent==null)
                {
                    ViewBag.msg = "You are not applied yet";
                    return View();
                }
                if (appliedStudent == null)
                {
                    ViewBag.msg = "You are not eligible";
                    return View();
                }
                var subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);

                SelectedStudentVM selectedStudent = new SelectedStudentVM() {
                    MeritStudent = meritStudent,
                    AppliedStudent = appliedStudent,
                    Subject = subject
                };

               
                if (selectedStudent != null)
                {
                    var admittedStudent =await _studentManager.GetStudentByHSCRollAsync(selectedStudent.MeritStudent.HSCRoll);
                    if (admittedStudent != null)
                    {
                        msg = "Congratulations! You are already admitted.";
                        ViewBag.admittedStudent = admittedStudent;
                        return View();
                    }
                    else
                    {

                        ViewBag.nuRoll = 3000012;
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
        [AllowAnonymous]
        public async Task<IActionResult> PaymentConfirmation(int NuAdmissionRoll, string notification)
        {
            string msg = "";
            if (NuAdmissionRoll > 0)
            {
                var meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(NuAdmissionRoll);
                var appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(NuAdmissionRoll);

                var subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);

                SelectedStudentVM selectedStudent = new SelectedStudentVM()
                {
                    MeritStudent = meritStudent,
                    AppliedStudent = appliedStudent,
                    Subject = subject
                };


                if (selectedStudent != null)
                {
                    var admittedStudent = await _studentManager.GetStudentByHSCRollAsync(selectedStudent.MeritStudent.HSCRoll);
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
        public async Task<ActionResult> CollegePayment(int nuRoll)
        {
            MeritStudent meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(nuRoll);
            Subject subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);
            AppliedStudent appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(nuRoll);
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

                return View(transactionInfo);
            }


            return View();
        }

        [HttpPost]
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

                await _paymentTransactionManager.AddAsync(newPayment);


                MeritStudent meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(model.PaymentTransaction.ReferenceNo);
                if (meritStudent != null)
                {
                    meritStudent.PaymentStatus = true;
                    PaymentTransaction paymentTransaction = await _paymentTransactionManager.GetTransactionByNuRollAsync(model.NuRoll);
                    if (paymentTransaction != null)
                    {
                        meritStudent.PaymentTransactionId = paymentTransaction.Id;
                        await _meritStudentManager.UpdateAsync(meritStudent);
                        TempData["msg"] = "Congratulations! Payment Successful.";
                        return RedirectToAction("Search", new { nuAdmissionRoll = model.PaymentTransaction.ReferenceNo });
                    }
                }
            }

            return View();
        }



        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<ActionResult> BkashPayment(int NuRoll)
        //{


        //    return View();
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<ActionResult> BkashPayment(SelectedStudentVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        MeritStudent mStudent = await _meritStudentManager.GetByAdmissionRollAsync(model.MeritStudent.NUAdmissionRoll);
        //        //mStudent.PaidAmaount = model.MeritStudent.PaidAmaount;
        //        mStudent.PaymentStatus = true;

        //    }

        //    return View(model);
        //}

        //[AllowAnonymous]
        //public ActionResult RocketPayment(int roll)
        //{
        //    return View();
        //}

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> NagadPayment(int nuRoll)
        {

            var meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(nuRoll);
            var subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);
            string OrderId = meritStudent.NUAdmissionRoll+""+meritStudent.SubjectCode+""+DateTime.Now.ToString("hhmmss");

            

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
                    httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.0.1");
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
                //Console.WriteLine("Signature Verification Failed");
                ViewBag.msg = "Signature Verification Failed";
                return View();
            }

            //Process Decrypted Data
            dynamic responsevalue = JObject.Parse(decryptedSensitiveData);
            string challenge = responsevalue.challenge;
            string paymentRefId = responsevalue.paymentReferenceId;
            //string amountOriginal = subject.AdmissionFee.ToString();
            //string amountWaiver = meritStudent.DeductedAmaount.ToString();
            //string amount = (Convert.ToInt32(amountOriginal) -Convert.ToInt32( amountWaiver)).ToString();
            string amount = (subject.AdmissionFee - meritStudent.DeductedAmaount).ToString();

            // Create JSON Object
            var paymentJSON = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = OrderId,
                currencyCode = "050",
                amount = amount,
                challenge = challenge
            };

            string paymentJsonData = JsonConvert.SerializeObject(paymentJSON);


            string paymentSensitiveData = EncryptWithPublic(paymentJsonData);

            // Generate Signature on JSON Data
            string paymentSignatureValue = SignWithMarchentPrivateKey(paymentJsonData);


            //string merchantCallbackURL = "http://sandbox.mynagad.com:10707/merchant-server/web/confirm"; //merchant Callback URL - as you want
            string merchantCallbackURL = "http://115.127.26.3:4356/api/PaymentTransactions"; //merchant Callback URL - as you want
            //string merchantCallbackURL = "https://localhost:44356/api/PaymentTransactions"; //merchant Callback URL - as you want

            // Prepare Final JSON for Payment API
            var paymentFinalJSON = new
            {
                sensitiveData = paymentSensitiveData,
                signature = paymentSignatureValue,
                merchantCallbackURL = merchantCallbackURL
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
                    br_httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.0.1");
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
                GlobalVariables.nuRoll = nuRoll.ToString();
                return Redirect(site);
                
            }

            //Console.WriteLine("Customer is redirecting to following Site:" + co_Response.callBackUrl + "\n");
            else
            {
                //Console.WriteLine("Failed");
                return View();
            }
                
            #endregion





            //MeritStudent meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(nuRoll);
            //Subject subject = await _subjectManager.GetByCodeAsync(meritStudent.SubjectCode);
            //AppliedStudent appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(nuRoll);
            //TransactionInfo transactionInfo = new TransactionInfo();

            //double admissinoFee = 0;
            //if (meritStudent != null)
            //{
            //    var deductionAmount = meritStudent.DeductedAmaount;
            //    if (deductionAmount >= 0)
            //    {
            //        admissinoFee = subject.AdmissionFee - deductionAmount;
            //    }
            //    else
            //    {
            //        admissinoFee = subject.AdmissionFee;
            //    }

            //    transactionInfo.Name = appliedStudent.ApplicantName;
            //    transactionInfo.NuRoll = meritStudent.NUAdmissionRoll;
            //    transactionInfo.Amount = admissinoFee;
            //    transactionInfo.SubjectName = subject.SubjectName;

            //    return View(transactionInfo);
            //}

        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> NagadPayment(TransactionInfo model)
        {
            
            if (ModelState.IsValid)
            {
                MeritStudent meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(model.PaymentTransaction.ReferenceNo);
                //HttpClient client = _api.Initial();
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
            var sub = await _subjectManager.GetByIdAsync(student.SubjectId);
            student.Subject = sub;
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
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

        #endregion

        #endregion




    }
}
