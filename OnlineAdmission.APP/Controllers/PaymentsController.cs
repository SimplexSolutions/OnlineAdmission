using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineAdmission.APP.ViewModels;
using OnlineAdmission.APP.ViewModels.PaymentsVM;
using OnlineAdmission.APP.ViewModels.Student;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.DB;
using OnlineAdmission.Entity;

namespace OnlineAdmission.APP.Controllers
{
    [Authorize(Roles ="Admin, SuperAdmin, Accounts")]
    public class PaymentsController : Controller
    {
        private readonly IPaymentTransactionManager _paymentTransactionManager;
        private readonly IStudentManager _studentManager;
        private readonly ISubjectManager _subjectManager;
        private readonly IMeritStudentManager _meritStudentManager;
        private readonly IAppliedStudentManager _appliedStudentManager;
        private readonly IStudentCategoryManager _studentCategoryManager;
        private readonly IPaymentTypeManager _paymentTypeManager;

        public PaymentsController(IPaymentTransactionManager paymentTransactionManager, IStudentManager studentManager, ISubjectManager subjectManager, IMeritStudentManager meritStudentManager, IAppliedStudentManager appliedStudentManager, IStudentCategoryManager studentCategoryManager, IPaymentTypeManager paymentTypeManager)
        {
            _paymentTransactionManager = paymentTransactionManager;
            _studentManager = studentManager;
            _subjectManager = subjectManager;
            _meritStudentManager = meritStudentManager;
            _appliedStudentManager = appliedStudentManager;
            _studentCategoryManager = studentCategoryManager;
            _paymentTypeManager = paymentTypeManager;
        }


        public async Task<IActionResult> Index(int studentCategoryId, int paymentTypeId, int page, int pageSize, string usrtext, DateTime? fromdate, DateTime? todate)
        {
            var students = _studentManager.GetIQueryableData();
            var appliedStudents = _appliedStudentManager.GetIQueryableData();
            var meritStudents = _meritStudentManager.GetIQueryableData();
            var paymentTransactions = _paymentTransactionManager.GetIQueryableData();
            var subjects = _subjectManager.GetIQueryableData();
            string pageTitle = "";
            if (studentCategoryId > 0)
            {
                ViewBag.studentCategoryId = studentCategoryId;
                StudentCategory studentCategory= await _studentCategoryManager.GetByIdAsync(studentCategoryId);
                pageTitle = studentCategory.CategoryName;
                students = students.Where(s => s.StudentCategoryId == studentCategoryId);
                appliedStudents = appliedStudents.Where(s => s.StudentCategoryId == studentCategoryId);
                meritStudents = meritStudents.Include(m => m.AcademicSession).Where(m => m.StudentCategoryId == studentCategoryId);

            }

            
            if (paymentTypeId>0)
            {
                ViewBag.paymentType = paymentTypeId;
                PaymentType paymentType = await _paymentTypeManager.GetByIdAsync(paymentTypeId);
                pageTitle = pageTitle + " (" + paymentType.PaymentTypeName + ")";
                paymentTransactions = paymentTransactions.Where(s => s.PaymentTypeId == paymentTypeId);
            }
            ViewBag.pageTitle = pageTitle;
            ViewBag.paymentTypes = new SelectList(await _paymentTypeManager.GetAllAsync(),"Id","PaymentTypeName", paymentTypeId);

            //IQueryable<PaymentReceiptVM> paymentReceiptVMs = from mStu in meritStudents
            //                                                 from aStu in appliedStudents.Where(a => a.NUAdmissionRoll == mStu.NUAdmissionRoll && a.AcademicSessionId == mStu.AcademicSessionId)
            //                                                 from stu in students.Where(a => a.NUAdmissionRoll == mStu.NUAdmissionRoll && a.AcademicSessionId == mStu.AcademicSessionId && a.Status == true)
            //                                                 from pt in paymentTransactions.Where(a => a.ReferenceNo == mStu.NUAdmissionRoll && a.AcademicSessionId == mStu.AcademicSessionId)
            //                                                 from sub in subjects.Where(s => s.Code == mStu.SubjectCode)
            //                                                 select new PaymentReceiptVM
            //                                                 {
            //                                                     PaymentTransaction = pt,
            //                                                     MeritStudent = mStu,
            //                                                     Subject = sub,
            //                                                     AppliedStudent = aStu,
            //                                                     AcademicSession = mStu.AcademicSession,
            //                                                     Student = stu
            //                                                 };

            IQueryable<PaymentReceiptVM> paymentReceiptVMs =    from pt in paymentTransactions
                                                                from mStu in meritStudents.Where(m => m.NUAdmissionRoll == pt.ReferenceNo)
                                                                from aStu in appliedStudents.Where(a => a.NUAdmissionRoll == pt.ReferenceNo && a.AcademicSessionId == pt.AcademicSessionId)
                                                                from stu in students.Where(s => s.AcademicSessionId == pt.AcademicSessionId && s.NUAdmissionRoll == pt.ReferenceNo && s.Status == true)
                                                                from sub in subjects.Where(s => s.Code == mStu.SubjectCode)
                                                                select new PaymentReceiptVM
                                                                {
                                                                    PaymentTransaction = pt,
                                                                    MeritStudent = mStu,
                                                                    Subject = sub,
                                                                    AppliedStudent = aStu,
                                                                    AcademicSession = mStu.AcademicSession,
                                                                    Student = stu
                                                                };



            //IQueryable<PaymentReceiptVM> paymentReceiptVMs = from pt in paymentTransactions

            //                                                 join s in students
            //                                                 on pt.ReferenceNo equals s.NUAdmissionRoll into ptsGroup
            //                                                 from stu in ptsGroup.DefaultIfEmpty()

            //                                                 join ms in meritStudents
            //                                                 on pt.ReferenceNo equals ms.NUAdmissionRoll into ptmsGroup
            //                                                 from mStu in ptmsGroup.DefaultIfEmpty()

            //                                                 join astu in appliedStudents
            //                                                 on pt.ReferenceNo equals astu.NUAdmissionRoll into ptastuGroup
            //                                                 from aStudent in ptastuGroup.DefaultIfEmpty()

            //                                                 select new PaymentReceiptVM
            //                                                 {
            //                                                     PaymentTransaction = pt,
            //                                                     Student = stu,
            //                                                     MeritStudent = mStu,
            //                                                     AppliedStudent = aStudent,
            //                                                     Subject = stu.Subject,
            //                                                     AcademicSession = mStu.AcademicSession
            //                                                 };

            ViewBag.controller = "Payments";
            ViewBag.action = "Index";
            ViewBag.data = usrtext;
            ViewBag.fromdate = fromdate;
            ViewBag.todate = todate;
            ViewBag.pagesize = pageSize;
            pageSize = pageSize <= 0 ? 50 : pageSize;
            page = page <= 0? 1: page;
            //var sss= DateTime.ParseExact(fromdate, "dd-MM-yyyy");
            //var  YearMonth = (DateTime)fromdate.ToString("") .ToString("dddd, yyyy-mm-dd");
            //((DateTime)fromdate).Date && a.PaymentTransaction.TransactionDate.Date >= ((DateTime)todate).Date) 
            
            if (fromdate != null || todate != null)
            {
                if (fromdate != null && todate != null)
                {
                    paymentReceiptVMs = from a in paymentReceiptVMs
                                        where (a.PaymentTransaction.TransactionDate>= fromdate && a.PaymentTransaction.TransactionDate <= todate) 
                                        select a;
                    //ViewBag.fromdate = ((DateTime)fromdate).Date.ToString("dd-MM-yyyy"); 
                    //ViewBag.todate = ((DateTime)todate).Date.ToString("dd-MM-yyyy"); 
                }
                else if (fromdate != null && todate == null)
                {
                    paymentReceiptVMs = from a in paymentReceiptVMs
                                        where (a.PaymentTransaction.TransactionDate.Date >= fromdate)
                                        select a;
                }
                else if (fromdate == null && todate != null)
                {
                    paymentReceiptVMs = from a in paymentReceiptVMs
                                        where (a.PaymentTransaction.TransactionDate.Date <= todate)
                                        select a;
                }
            }
            //else if (fromdate == null && todate == null)
            //{
            //    fromdate = DateTime.Now.AddDays(-10);
            //    todate = DateTime.Now;
            //    paymentReceiptVMs = from a in paymentReceiptVMs
            //                        where (a.PaymentTransaction.TransactionDate >= fromdate && a.PaymentTransaction.TransactionDate <= todate)
            //                        select a;
            //}

            if (paymentTypeId ==2)
            {
                paymentReceiptVMs = from a in paymentReceiptVMs
                                    from sub in subjects.Where(s => s.Code == a.MeritStudent.SubjectCode)
                                    select a;
            }

            if (!string.IsNullOrEmpty(usrtext))
            {
                usrtext = usrtext.Trim().ToLower();

                paymentReceiptVMs = paymentReceiptVMs.Where(m => m.AppliedStudent.ApplicantName.ToLower().Contains(usrtext) || m.PaymentTransaction.AccountNo.ToLower() == usrtext || m.PaymentTransaction.TransactionId.ToLower() == usrtext || m.Student.CollegeRoll.ToString() == usrtext || m.Student.StudentMobile.ToString().Contains(usrtext) || m.AppliedStudent.NUAdmissionRoll.ToString().ToLower() == usrtext || m.Subject.SubjectName.ToLower() == usrtext || m.PaymentTransaction.Amount.ToString().ToLower() == usrtext || m.PaymentTransaction.TransactionDate.ToString().Contains(usrtext));
            }
            ViewBag.count = paymentReceiptVMs.Count();
            if (pageSize == 5001)
            {
                pageSize = paymentReceiptVMs.Count();
            }
            return View(await PaginatedList<PaymentReceiptVM>.CreateAsync(paymentReceiptVMs, page, pageSize));
        }
        // GET: Payments
        
        public async Task<IActionResult> ProfessionalIndex(string usrtext, string sortRoll, int page, int pagesize, DateTime? fromdate, DateTime? todate, int paymentType)
        {
            ViewBag.action = "ProfessionalIndex";
            ViewBag.controller = "Payments";
            ViewBag.paymentType = paymentType;
            if (paymentType == 1)
            {
                ViewBag.pageTitle = "Hons Professional Application Payment";
            }
            else
            {
                ViewBag.pageTitle = "Hons Professional Admission Payment";
            }

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
            }

            IQueryable<PaymentTransaction> paymentTransactions = _paymentTransactionManager.GetIQueryableData().Where(p => p.StudentCategoryId == 2 && p.PaymentTypeId==paymentType);


            ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : " ";


            switch (sortRoll)
            {
                case "desc":
                    paymentTransactions = paymentTransactions.OrderByDescending(m => m.ReferenceNo);
                    break;
                default:
                    paymentTransactions = paymentTransactions.OrderBy(m => m.ReferenceNo);
                    break;
            }
            ViewBag.data = usrtext;

            int pageSize = pagesize <= 0 ? 50 : pagesize;
            if (page <= 0) page = 1;

            if (fromdate != null || todate != null)
            {
                if (fromdate != null && todate != null)
                {
                    paymentTransactions = from a in paymentTransactions
                                          where (a.TransactionDate.Date >= fromdate && a.TransactionDate.Date <= todate)
                                        select a;
                }
                else if (fromdate != null && todate == null)
                {
                    paymentTransactions = from a in paymentTransactions
                                          where (a.TransactionDate.Date >= fromdate)
                                        select a;
                }
                else if (fromdate == null && todate != null)
                {
                    paymentTransactions = from a in paymentTransactions
                                          where (a.TransactionDate.Date <= todate)
                                        select a;
                }
            }

            if (!string.IsNullOrEmpty(usrtext))
            {
                usrtext = usrtext.Trim().ToLower();

                paymentTransactions = paymentTransactions.Where(m => m.StudentName.ToLower().Contains(usrtext) || m.AccountNo.ToLower() == usrtext || m.TransactionId.ToLower() == usrtext || m.ReferenceNo.ToString().ToLower() == usrtext || m.Amount.ToString().ToLower() == usrtext || m.TransactionDate.ToString().Contains(usrtext));
                ViewBag.count = paymentTransactions.Count();

                return View(await PaginatedList<PaymentTransaction>.CreateAsync(paymentTransactions, page, pageSize));
            }
            ViewBag.count = paymentTransactions.Count();
            return View(await PaginatedList<PaymentTransaction>.CreateAsync(paymentTransactions, page, pageSize));

        }
        
        public async Task<IActionResult> MastersIndex(string usrtext, string sortRoll, int page, int pagesize, DateTime? fromdate, DateTime? todate, int paymentType)
        {
            ViewBag.action = "mastersIndex";
            ViewBag.controller = "Payments";
            ViewBag.paymentType = paymentType;
            if (paymentType == 1)
            {
                ViewBag.pageTitle = "Masters Professional(MBA) Application Payment";
            }
            else
            {
                ViewBag.pageTitle = "Masters Professional(MBA) Admission Payment";
            }

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
            }

            IQueryable<PaymentTransaction> paymentTransactions = _paymentTransactionManager.GetIQueryableData().Where(p => p.StudentCategoryId == 3 && p.PaymentTypeId==paymentType);


            ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : " ";


            switch (sortRoll)
            {
                case "desc":
                    paymentTransactions = paymentTransactions.OrderByDescending(m => m.ReferenceNo);
                    break;
                default:
                    paymentTransactions = paymentTransactions.OrderBy(m => m.ReferenceNo);
                    break;
            }
            ViewBag.data = usrtext;

            int pageSize = pagesize <= 0 ? 50 : pagesize;
            if (page <= 0) page = 1;

            if (fromdate != null || todate != null)
            {
                if (fromdate != null && todate != null)
                {
                    paymentTransactions = from a in paymentTransactions
                                          where (a.TransactionDate.Date >= fromdate && a.TransactionDate.Date <= todate)
                                        select a;
                }
                else if (fromdate != null && todate == null)
                {
                    paymentTransactions = from a in paymentTransactions
                                          where (a.TransactionDate.Date >= fromdate)
                                        select a;
                }
                else if (fromdate == null && todate != null)
                {
                    paymentTransactions = from a in paymentTransactions
                                          where (a.TransactionDate.Date <= todate)
                                        select a;
                }
            }

            if (!string.IsNullOrEmpty(usrtext))
            {
                usrtext = usrtext.Trim().ToLower();

                paymentTransactions = paymentTransactions.Where(m => m.StudentName.ToLower().Contains(usrtext) || m.AccountNo.ToLower() == usrtext 
                || m.TransactionId.ToLower() == usrtext || m.ReferenceNo.ToString().ToLower() == usrtext 
                || m.Amount.ToString().ToLower() == usrtext || m.TransactionDate.ToString().Contains(usrtext));
                ViewBag.count = paymentTransactions.Count();

                return View(await PaginatedList<PaymentTransaction>.CreateAsync(paymentTransactions, page, pageSize));
            }
            ViewBag.count = paymentTransactions.Count();
            return View(await PaginatedList<PaymentTransaction>.CreateAsync(paymentTransactions, page, pageSize));

        }
        
        public async Task<IActionResult> MastersGeneralIndex(string usrtext, string sortRoll, int page, int pagesize, DateTime? fromdate, DateTime? todate, int paymentType)
        {
            ViewBag.action = "mastersGeneralIndex";
            ViewBag.controller = "Payments";
            ViewBag.paymentType = paymentType;
            if (paymentType == 1)
            {
                ViewBag.pageTitle = "Masters (General) Application Payment";
            }
            else
            {
                ViewBag.pageTitle = "Masters (General) Admission Payment";
            }

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
            }

            IQueryable<PaymentReceiptVM> paymentReceiptVMs = from t in _paymentTransactionManager.GetIQueryableData().Where(a => a.StudentCategoryId == 4 && a.PaymentTypeId == paymentType)
                                                             from m in _meritStudentManager.GetIQueryableData().Where(a => a.NUAdmissionRoll == t.ReferenceNo && a.PaymentStatus == true)
                                                             from sub in _subjectManager.GetIQueryableData().Where(a => a.Code == m.SubjectCode)
                                                             from s in _appliedStudentManager.GetIQueryableData().Where(a => a.NUAdmissionRoll == t.ReferenceNo)
                                                             join stu in _studentManager.GetIQueryableData().Where(a => a.Status == true) on m.NUAdmissionRoll equals stu.NUAdmissionRoll into myList
                                                             from subList in myList.DefaultIfEmpty()
                                                             select new PaymentReceiptVM
                                                             {
                                                                 PaymentTransaction = t,
                                                                 MeritStudent = m,
                                                                 Subject = sub,
                                                                 AppliedStudent = s,
                                                                 Student = subList
                                                             };



            //IQueryable<PaymentTransaction> paymentTransactions = _paymentTransactionManager.GetIQueryableData().Where(p => p.StudentCategory == 4 && p.PaymentType==paymentType);


            ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : " ";


            switch (sortRoll)
            {
                case "desc":
                    paymentReceiptVMs = paymentReceiptVMs.OrderByDescending(m => m.PaymentTransaction.ReferenceNo);
                    break;
                default:
                    paymentReceiptVMs = paymentReceiptVMs.OrderBy(m => m.PaymentTransaction.ReferenceNo);
                    break;
            }
            ViewBag.data = usrtext;

            int pageSize = pagesize <= 0 ? 50 : pagesize;
            if (page <= 0) page = 1;

            if (fromdate != null || todate != null)
            {
                if (fromdate != null && todate != null)
                {
                    paymentReceiptVMs = from a in paymentReceiptVMs
                                        where (a.PaymentTransaction.TransactionDate.Date >= fromdate && a.PaymentTransaction.TransactionDate.Date <= todate)
                                        select a;
                }
                else if (fromdate != null && todate == null)
                {
                    paymentReceiptVMs = from a in paymentReceiptVMs
                                        where (a.PaymentTransaction.TransactionDate.Date >= fromdate)
                                        select a;
                }
                else if (fromdate == null && todate != null)
                {
                    paymentReceiptVMs = from a in paymentReceiptVMs
                                        where (a.PaymentTransaction.TransactionDate.Date <= todate)
                                        select a;
                }
            }

            if (!string.IsNullOrEmpty(usrtext))
            {
                usrtext = usrtext.Trim().ToLower();

                paymentReceiptVMs = paymentReceiptVMs.Where(m => m.Student.Name.ToLower().Contains(usrtext) || m.PaymentTransaction.AccountNo.ToLower() == usrtext 
                || m.PaymentTransaction.TransactionId.ToLower() == usrtext || m.PaymentTransaction.ReferenceNo.ToString().ToLower() == usrtext 
                || m.PaymentTransaction.Amount.ToString().ToLower() == usrtext || m.PaymentTransaction.TransactionDate.ToString().Contains(usrtext));
                
                ViewBag.count = paymentReceiptVMs.Count();

                return View(await PaginatedList<PaymentReceiptVM>.CreateAsync(paymentReceiptVMs, page, pageSize));
            }
            ViewBag.count = paymentReceiptVMs.Count();
            return View(await PaginatedList<PaymentReceiptVM>.CreateAsync(paymentReceiptVMs, page, pageSize));

        }
        public async Task<IActionResult> DegreeIndex(string usrtext, string sortRoll, int page, int pagesize, DateTime? fromdate, DateTime? todate, int paymentType)
        {
            ViewBag.action = "degreeindex";
            ViewBag.controller = "Payments";
            ViewBag.paymentType = paymentType;
            if (paymentType == 1)
            {
                ViewBag.pageTitle = "Degree Pass Application Payment";
            }
            else
            {
                ViewBag.pageTitle = "Degree Pass Admission Payment";
            }

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
            }

            IQueryable<PaymentTransaction> paymentTransactions = _paymentTransactionManager.GetIQueryableData().Where(p => p.StudentCategoryId == 5 && p.PaymentTypeId == paymentType);


            ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : " ";


            switch (sortRoll)
            {
                case "desc":
                    paymentTransactions = paymentTransactions.OrderByDescending(m => m.ReferenceNo);
                    break;
                default:
                    paymentTransactions = paymentTransactions.OrderBy(m => m.ReferenceNo);
                    break;
            }
            ViewBag.data = usrtext;

            int pageSize = pagesize <= 0 ? 50 : pagesize;
            if (page <= 0) page = 1;

            if (fromdate != null || todate != null)
            {
                if (fromdate != null && todate != null)
                {
                    paymentTransactions = from a in paymentTransactions
                                          where (a.TransactionDate.Date >= fromdate && a.TransactionDate.Date <= todate)
                                          select a;
                }
                else if (fromdate != null && todate == null)
                {
                    paymentTransactions = from a in paymentTransactions
                                          where (a.TransactionDate.Date >= fromdate)
                                          select a;
                }
                else if (fromdate == null && todate != null)
                {
                    paymentTransactions = from a in paymentTransactions
                                          where (a.TransactionDate.Date <= todate)
                                          select a;
                }
            }

            if (!string.IsNullOrEmpty(usrtext))
            {
                usrtext = usrtext.Trim().ToLower();

                paymentTransactions = paymentTransactions.Where(m => m.StudentName.ToLower().Contains(usrtext) || m.AccountNo.ToLower() == usrtext
                || m.TransactionId.ToLower() == usrtext || m.ReferenceNo.ToString().ToLower() == usrtext
                || m.Amount.ToString().ToLower() == usrtext || m.TransactionDate.ToString().Contains(usrtext));
                ViewBag.count = paymentTransactions.Count();

                return View(await PaginatedList<PaymentTransaction>.CreateAsync(paymentTransactions, page, pageSize));
            }
            ViewBag.count = paymentTransactions.Count();
            return View(await PaginatedList<PaymentTransaction>.CreateAsync(paymentTransactions, page, pageSize));

        }


        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id, int studentCategoryId, int academicSessionId, int meritTypeId)
        {
            if (id == null)
            {
                return NotFound();
            }
            var allMeritStudents = await _meritStudentManager.GetAllAsync();

            var paymentTransaction = await _paymentTransactionManager.GetByIdAsync((int)id);
            StudentCategory studentCategory = await _studentCategoryManager.GetByIdAsync(studentCategoryId);
            ViewBag.studentCategoryId = studentCategoryId;
            MeritStudent existMeritStudent = await _meritStudentManager.GetMeritStudentAsync(paymentTransaction.ReferenceNo, studentCategoryId, meritTypeId, academicSessionId);

            var appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(paymentTransaction.ReferenceNo, studentCategoryId);
            Subject existSubject = new Subject();
            Student student = new Student();
            if (existMeritStudent!=null)
            {
                existSubject = await _subjectManager.GetByCodeAsync(existMeritStudent.SubjectCode);
                student = await _studentManager.GetByAdmissionRollAsync(existMeritStudent.NUAdmissionRoll, studentCategoryId);
            }

            PaymentReceiptVM paymentReceiptVM = new PaymentReceiptVM();
            paymentReceiptVM.PaymentTransaction = paymentTransaction;
            paymentReceiptVM.MeritStudent = existMeritStudent;
            if (appliedStudent!=null)
            {
                paymentReceiptVM.AppliedStudent = appliedStudent;
            }
            paymentReceiptVM.Subject = existSubject;
            if (student!=null)
            {
                paymentReceiptVM.Student = student;
                
            }
            

            return View(paymentReceiptVM);
        }

        // GET: Payments/Create
        [Authorize(Roles = "SuperAdmin, Admin")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Create( PaymentTransaction paymentTransaction)
        {
            if (ModelState.IsValid)
            {
                await _paymentTransactionManager.AddAsync(paymentTransaction);
                var meritStudent =await _meritStudentManager.GetHonsByAdmissionRollAsync(paymentTransaction.ReferenceNo);
                meritStudent.PaymentStatus = true;
                meritStudent.PaymentTransactionId = paymentTransaction.Id;
                bool isSaved = await _meritStudentManager.UpdateAsync(meritStudent);
                if (isSaved)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(paymentTransaction);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentTransaction = await _paymentTransactionManager.GetByIdAsync((int)id);
            if (paymentTransaction == null)
            {
                return NotFound();
            }
            return View(paymentTransaction);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,TransactionDate,Balance,AccountNo,TransactionId,ReferenceNo,AdmissionFee,ServiceCharge")] PaymentTransaction paymentTransaction)
        {
            if (id != paymentTransaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _paymentTransactionManager.UpdateAsync(paymentTransaction);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentTransactionExists(paymentTransaction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(paymentTransaction);
        }

        [HttpGet]
        public async Task<IActionResult> SummeryReport()
        {
            List<PaymentTransaction> payments = await _paymentTransactionManager.GetAllAsync();            
            return View(payments);
        }

        [HttpPost]
        public async Task<IActionResult> SummeryReport(DateTime? fromDate, DateTime? toDate)
        {
            ViewBag.FromDate =Convert.ToDateTime(fromDate).Date.ToShortDateString();
            ViewBag.ToDate =Convert.ToDateTime(toDate).Date.ToShortDateString();
            List<PaymentTransaction> payments = await _paymentTransactionManager.GetAllAsync();

            if (fromDate != null && toDate != null)
            {
                payments = payments.Where(p => p.TransactionDate.Date >= (DateTime)fromDate && p.TransactionDate.Date <= (DateTime)toDate).ToList();
            }
            else if (fromDate == null && toDate != null)
            {
                payments = payments.Where(p => p.TransactionDate.Date <= (DateTime)toDate).ToList();
            }
            else if (fromDate != null && toDate == null)
            {
                payments = payments.Where(p => p.TransactionDate.Date >= (DateTime)fromDate).ToList();
            }
            else
            {
                payments = new List<PaymentTransaction>();
            }


            ViewBag.totalPaymentCount = payments.Count();
            ViewBag.applicationPaymentCount = payments.Where(s => s.PaymentTypeId == 2).Count();
            ViewBag.admissionPaymentCount = payments.Where(s => s.PaymentTypeId == 1).Count();

            ViewBag.totalApplicationPayment = payments.Where(s => s.PaymentTypeId == 2).Sum(s => s.Amount);
            ViewBag.totalApplicationPaymentService = payments.Where(s => s.PaymentTypeId == 2).Sum(s => s.ServiceCharge);
            ViewBag.totalApplicationPaymentNet = payments.Where(s => s.PaymentTypeId == 2).Sum(s => s.Amount)- ViewBag.totalApplicationPaymentService;

            ViewBag.totalAdmissionPayment = payments.Where(s => s.PaymentTypeId == 1).Sum(s => s.Amount);
            ViewBag.totalAdmissionPaymentService = payments.Where(s => s.PaymentTypeId == 1).Sum(s => s.ServiceCharge);
            ViewBag.totalAdmissionPaymentNet = payments.Where(s => s.PaymentTypeId == 1).Sum(s => s.Amount) - ViewBag.totalAdmissionPaymentService;

            ViewBag.totalPayment = payments.Sum(s => s.Amount);
            ViewBag.totalPaymentService = payments.Sum(s => s.ServiceCharge);
            ViewBag.totalPaymentNet = payments.Sum(s => s.Amount) - payments.Sum(s => s.ServiceCharge);

            /////////////////// APPLICATION PAYMENT START ///////////////////////////////////

            ViewBag.honorsApplicationPaymentCount = payments.Where(p => p.StudentCategoryId == 1 && p.PaymentTypeId == 1).Count();
            ViewBag.honorsApplicationPaymentSum = payments.Where(p => p.StudentCategoryId == 1 && p.PaymentTypeId == 1).Sum(m => m.Amount);
            ViewBag.honorsApplicationPaymentService = payments.Where(p => p.StudentCategoryId == 1 && p.PaymentTypeId == 1).Sum(m => m.ServiceCharge);
            ViewBag.honorsApplicationPaymentSumNet = payments.Where(p => p.StudentCategoryId == 1 && p.PaymentTypeId == 1).Sum(m => m.Amount) - payments.Where(p => p.StudentCategoryId == 1 && p.PaymentTypeId == 1).Sum(m => m.ServiceCharge);

            ViewBag.honorsProApplicationPaymentCount = payments.Where(p => p.StudentCategoryId == 2 && p.PaymentTypeId == 1).Count();
            ViewBag.honorsProApplicationPaymentSum = payments.Where(p => p.StudentCategoryId == 2 && p.PaymentTypeId == 1).Sum(m => m.Amount);
            ViewBag.honorsProApplicationPaymentService = payments.Where(p => p.StudentCategoryId == 2 && p.PaymentTypeId == 1).Sum(m => m.ServiceCharge);
            ViewBag.honorsProApplicationPaymentSumNet = payments.Where(p => p.StudentCategoryId == 2 && p.PaymentTypeId == 1).Sum(m => m.Amount) - payments.Where(p => p.StudentCategoryId == 2 && p.PaymentTypeId == 1).Sum(m => m.ServiceCharge);
                        
            ViewBag.mastersProApplicationPaymentCount = payments.Where(p => p.StudentCategoryId == 3 && p.PaymentTypeId == 1).Count();
            ViewBag.mastersProApplicationPaymentSum = payments.Where(p => p.StudentCategoryId == 3 && p.PaymentTypeId == 1).Sum(p => p.Amount);
            ViewBag.mastersProApplicationPaymentService = payments.Where(p => p.StudentCategoryId == 3 && p.PaymentTypeId == 1).Sum(p => p.ServiceCharge);
            ViewBag.mastersProApplicationPaymentSumNet = payments.Where(p => p.StudentCategoryId == 3 && p.PaymentTypeId == 1).Sum(p => p.Amount)- payments.Where(p => p.StudentCategoryId == 3 && p.PaymentTypeId == 1).Sum(p => p.ServiceCharge);

            ViewBag.mastersApplicationPaymentCount = payments.Where(p => p.StudentCategoryId == 4 && p.PaymentTypeId == 1).Count();
            ViewBag.mastersApplicationPaymentSum = payments.Where(p => p.StudentCategoryId == 4 && p.PaymentTypeId == 1).Sum(p => p.Amount);
            ViewBag.mastersApplicationPaymentService = payments.Where(p => p.StudentCategoryId == 4 && p.PaymentTypeId == 1).Sum(p => p.ServiceCharge);
            ViewBag.mastersApplicationPaymentSumNet = payments.Where(p => p.StudentCategoryId == 4 && p.PaymentTypeId == 1).Sum(p => p.Amount) - payments.Where(p => p.StudentCategoryId == 4 && p.PaymentTypeId == 1).Sum(p => p.ServiceCharge);


            ViewBag.degreeApplicationPaymentCount = payments.Where(p => p.StudentCategoryId == 5 && p.PaymentTypeId == 1).Count();
            ViewBag.degreeApplicationPaymentSum = payments.Where(p => p.StudentCategoryId == 5 && p.PaymentTypeId == 1).Sum(p => p.Amount);
            ViewBag.degreeApplicationPaymentService = payments.Where(p => p.StudentCategoryId == 5 && p.PaymentTypeId == 1).Sum(p => p.ServiceCharge);
            ViewBag.degreeApplicationPaymentSumNet = payments.Where(p => p.StudentCategoryId == 5 && p.PaymentTypeId == 1).Sum(p => p.Amount) - payments.Where(p => p.StudentCategoryId == 5 && p.PaymentTypeId == 1).Sum(p => p.ServiceCharge);

            /////////////////// APPLICATION PAYMENT END ///////////////////////////////////

            /*----------------------------------------------------------------------------------------------*/


            /////////////////// ADMISSION PAYMENT START //////////////////////////////////

            ViewBag.honorsAdmissionPaymentCount = payments.Where(p => p.StudentCategoryId == 1 && p.PaymentTypeId == 2).Count();
            ViewBag.honorsAdmissionPaymentSum = payments.Where(p => p.StudentCategoryId == 1 && p.PaymentTypeId == 2).Sum(p => p.Amount);
            ViewBag.honorsAdmissionPaymentService = payments.Where(p => p.StudentCategoryId == 1 && p.PaymentTypeId == 2).Sum(p => p.ServiceCharge);
            ViewBag.honorsAdmissionPaymentSumNet = payments.Where(p => p.StudentCategoryId == 1 && p.PaymentTypeId == 2).Sum(p => p.Amount) - payments.Where(p => p.StudentCategoryId == 1 && p.PaymentTypeId == 2).Sum(p => p.ServiceCharge);

            ViewBag.honorsProAdmissionPaymentCount = payments.Where(p => p.StudentCategoryId == 2 && p.PaymentTypeId == 2).Count();
           // ViewBag.honorsProAdmissionPaymentService = payments.Where(p => p.StudentCategory == 2 && p.PaymentType == 2).Sum(p => p.Amount);
            ViewBag.honorsProAdmissionPaymentSum = payments.Where(p => p.StudentCategoryId == 2 && p.PaymentTypeId == 2).Sum(p => p.Amount);
            ViewBag.honorsProAdmissionPaymentService = payments.Where(p => p.StudentCategoryId == 2 && p.PaymentTypeId == 2).Sum(p => p.ServiceCharge);
            ViewBag.honorsProAdmissionPaymentSumNet = payments.Where(p => p.StudentCategoryId == 2 && p.PaymentTypeId == 2).Sum(p => p.Amount) - payments.Where(p => p.StudentCategoryId == 2 && p.PaymentTypeId == 2).Sum(p => p.ServiceCharge);

            ViewBag.mastersProAdmissionPaymentCount = payments.Where(p => p.StudentCategoryId == 3 && p.PaymentTypeId == 2).Count();
            ViewBag.mastersProAdmissionPaymentSum = payments.Where(p => p.StudentCategoryId == 3 && p.PaymentTypeId == 2).Sum(p => p.Amount);
            ViewBag.mastersProAdmissionPaymentService = payments.Where(p => p.StudentCategoryId == 3 && p.PaymentTypeId == 2).Sum(p => p.ServiceCharge);
            ViewBag.mastersProAdmissionPaymentSumNet = payments.Where(p => p.StudentCategoryId == 3 && p.PaymentTypeId == 2).Sum(p => p.Amount) - payments.Where(p => p.StudentCategoryId == 3 && p.PaymentTypeId == 2).Sum(p => p.ServiceCharge);

            ViewBag.mastersAdmissionPaymentCount = payments.Where(p => p.StudentCategoryId == 4 && p.PaymentTypeId == 2).Count();
            ViewBag.mastersAdmissionPaymentSum = payments.Where(p => p.StudentCategoryId == 4 && p.PaymentTypeId == 2).Sum(p => p.Amount);
            ViewBag.mastersAdmissionPaymentService = payments.Where(p => p.StudentCategoryId == 4 && p.PaymentTypeId == 2).Sum(p => p.ServiceCharge);
            ViewBag.mastersAdmissionPaymentSumNet = payments.Where(p => p.StudentCategoryId == 4 && p.PaymentTypeId == 2).Sum(p => p.Amount) - payments.Where(p => p.StudentCategoryId == 4 && p.PaymentTypeId == 2).Sum(p => p.ServiceCharge);

            ViewBag.degreeAdmissionPaymentCount = payments.Where(p => p.StudentCategoryId == 5 && p.PaymentTypeId == 2).Count();
            ViewBag.degreeAdmissionPaymentSum = payments.Where(p => p.StudentCategoryId == 5 && p.PaymentTypeId == 2).Sum(p => p.Amount);
            ViewBag.degreeAdmissionPaymentService = payments.Where(p => p.StudentCategoryId == 5 && p.PaymentTypeId == 2).Sum(p => p.Amount);
            ViewBag.degreeAdmissionPaymentSumNet = payments.Where(p => p.StudentCategoryId == 5 && p.PaymentTypeId == 2).Sum(p => p.Amount) - payments.Where(p => p.StudentCategoryId == 5 && p.PaymentTypeId == 2).Sum(p => p.Amount);

            /////////////////// ADMISSION PAYMENT END //////////////////////////////////



            return View(payments);
        }

        [HttpPost]
        public async Task<IActionResult> SummeryReport1(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate != null)
            {
                ViewBag.FromDate = Convert.ToDateTime(fromDate).ToString("dd-MMM-yyyy");
            }
            if (toDate != null)
            {
                ViewBag.ToDate = Convert.ToDateTime(toDate).ToString("dd-MMM-yyyy");
            }
            List<PaymentTransaction> payments = await _paymentTransactionManager.GetAllAsync();
            List<StudentCategory> categories = await _studentCategoryManager.GetAllAsync();

            if (fromDate != null && toDate != null)
            {
                payments = payments.Where(p => p.TransactionDate.Date >= (DateTime)fromDate && p.TransactionDate.Date <= (DateTime)toDate).ToList();
            }
            else if (fromDate == null && toDate != null)
            {
                payments = payments.Where(p => p.TransactionDate.Date <= (DateTime)toDate).ToList();
            }
            else if (fromDate != null && toDate == null)
            {
                payments = payments.Where(p => p.TransactionDate.Date >= (DateTime)fromDate).ToList();
            }
            else
            {
                payments = new List<PaymentTransaction>();
            }

            List<PaymentType> paymentTypes = await _paymentTypeManager.GetAllAsync();

            SummeryReportVM summeryReportVM = new SummeryReportVM();
            summeryReportVM.PaymentTransactions = payments;
            summeryReportVM.StudentCategories = categories;
            summeryReportVM.PaymentTypes = paymentTypes;


            return View(summeryReportVM);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentTransaction = await _paymentTransactionManager.GetByIdAsync((int)id);
            if (paymentTransaction == null)
            {
                return NotFound();
            }

            return View(paymentTransaction);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentTransaction = await _paymentTransactionManager.GetByIdAsync(id);
            await _paymentTransactionManager.RemoveAsync(paymentTransaction);

            return RedirectToAction(nameof(Index));
        }

        private bool PaymentTransactionExists(int id)
        {
            var payment = _paymentTransactionManager.GetByIdAsync(id);
            if (payment!=null)
            {
                return true;
            }
            return false;
        }
    }
}
