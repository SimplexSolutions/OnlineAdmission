using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineAdmission.APP.ViewModels;
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

        public PaymentsController(IPaymentTransactionManager paymentTransactionManager, IStudentManager studentManager, ISubjectManager subjectManager, IMeritStudentManager meritStudentManager, IAppliedStudentManager appliedStudentManager)
        {
            _paymentTransactionManager = paymentTransactionManager;
            _studentManager = studentManager;
            _subjectManager = subjectManager;
            _meritStudentManager = meritStudentManager;
            _appliedStudentManager = appliedStudentManager;
        }

        // GET: Payments
        public async Task<IActionResult> Index(string usrtext, string sortRoll, string sortNURoll, int page, int pagesize, DateTime? fromdate, DateTime? todate)
        {
            ViewBag.action = "Index";
            ViewBag.controller = "Payments";

            if (TempData["msg"]!=null)
            {
                ViewBag.msg = TempData["msg"].ToString();
            }


            IQueryable<PaymentReceiptVM> paymentReceiptVMs = from t in _paymentTransactionManager.GetIQueryableData()
                                                             from m in _meritStudentManager.GetIQueryableData().Where(a => a.NUAdmissionRoll==t.ReferenceNo && a.PaymentStatus==true)
                                                             from sub in _subjectManager.GetIQueryableData().Where(a => a.Code == m.SubjectCode)
                                                             from s in _appliedStudentManager.GetIQueryableData().Where(a=>a.NUAdmissionRoll == t.ReferenceNo)
                                                             join stu in _studentManager.GetIQueryableData().Where(a=>a.Status==true) on m.NUAdmissionRoll equals stu.NUAdmissionRoll  into myList
                                                             from subList in myList.DefaultIfEmpty()
                                                             select new PaymentReceiptVM{
                                                                 PaymentTransaction = t,
                                                                 MeritStudent = m,
                                                                 Subject = sub,
                                                                 AppliedStudent = s,
                                                                 Student = subList
                                                             };
            //paymentReceiptVMs = paymentReceiptVMs.OrderBy(m=>m.PaymentTransaction.ReferenceNo);
            if (sortRoll == null && sortNURoll == null)
            {
                ViewBag.sortNURoll = string.IsNullOrEmpty(sortNURoll) ? "desc" : "";
                ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : "";
                switch (sortNURoll)
                {
                    case "desc":
                        paymentReceiptVMs = paymentReceiptVMs.OrderByDescending(m => m.PaymentTransaction.ReferenceNo);
                        break;
                    default:
                        paymentReceiptVMs = paymentReceiptVMs.OrderBy(m => m.PaymentTransaction.ReferenceNo);
                        break;
                }
            }
            else if (sortNURoll == "desc" && sortRoll == null)
            {
                ViewBag.sortNURoll = string.IsNullOrEmpty(sortNURoll) ? "desc" : "";
                ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : "";
                switch (sortNURoll)
                {
                    case "desc":
                        paymentReceiptVMs = paymentReceiptVMs.OrderByDescending(m => m.PaymentTransaction.ReferenceNo);
                        break;
                    default:
                        paymentReceiptVMs = paymentReceiptVMs.OrderBy(m => m.PaymentTransaction.ReferenceNo);
                        break;
                }
                //switch (sortRoll)
                //{
                //    case "desc":
                //        paymentReceiptVMs = paymentReceiptVMs.OrderByDescending(m => m.Student.CollegeRoll);
                //        break;
                //    default:
                //        paymentReceiptVMs = paymentReceiptVMs.OrderBy(m => m.Student.CollegeRoll);
                //        break;
                //}
            }
            else if (sortNURoll == "desc")
            {
                ViewBag.sortNURoll = string.IsNullOrEmpty(sortNURoll) ? "desc" : "";
                ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : "";
                switch (sortNURoll)
                {
                    case "desc":
                        paymentReceiptVMs = paymentReceiptVMs.OrderByDescending(m => m.PaymentTransaction.ReferenceNo);
                        break;
                    default:
                        paymentReceiptVMs = paymentReceiptVMs.OrderBy(m => m.PaymentTransaction.ReferenceNo);
                        break;
                }
            }
            else if (sortRoll == "desc" || sortRoll == "asc")
            {
                ViewBag.sortNURoll = string.IsNullOrEmpty(sortNURoll) ? "desc" : "";
                switch (sortRoll)
                {
                    case "desc":
                        paymentReceiptVMs = paymentReceiptVMs.OrderByDescending(m => m.Student.CollegeRoll);
                        ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : "asc";
                        break;
                    default:
                        paymentReceiptVMs = paymentReceiptVMs.OrderBy(m => m.Student.CollegeRoll);
                        ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "asc" : "desc";
                        break;
                }
            }
            else
            {
                ViewBag.sortNURoll = string.IsNullOrEmpty(sortNURoll) ? "desc" : "";
                ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : "";
                switch (sortRoll)
                {
                    case "desc":
                        paymentReceiptVMs = paymentReceiptVMs.OrderByDescending(m => m.Student.CollegeRoll);
                        break;
                    default:
                        paymentReceiptVMs = paymentReceiptVMs.OrderBy(m => m.Student.CollegeRoll);
                        break;
                }

            }
                         
            
 
           

            

            ViewBag.data = usrtext;
            ViewBag.fromdate = fromdate;
            ViewBag.todate = todate;
            int pageSize = pagesize <= 0 ? 50 : pagesize;
            
            if (page <= 0) page = 1;

            if (pageSize == 5001)
            {
                pageSize = paymentReceiptVMs.Count();
            }
            ViewBag.pagesize = pageSize;
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
                if (pageSize == 5001)
                {
                    pageSize = paymentReceiptVMs.Count();
                }

            }

            if (!string.IsNullOrEmpty(usrtext))
            {
                usrtext = usrtext.Trim().ToLower();

                paymentReceiptVMs = paymentReceiptVMs.Where(m => m.AppliedStudent.ApplicantName.ToLower().Contains(usrtext) || m.PaymentTransaction.AccountNo.ToLower() == usrtext || m.PaymentTransaction.TransactionId.ToLower() == usrtext || m.AppliedStudent.NUAdmissionRoll.ToString().ToLower() == usrtext || m.Subject.SubjectName.ToLower() == usrtext || m.PaymentTransaction.Amount.ToString().ToLower() == usrtext || m.PaymentTransaction.TransactionDate.ToString().Contains(usrtext));
                ViewBag.count = paymentReceiptVMs.Count();
                if (pageSize == 5001)
                {
                    pageSize = paymentReceiptVMs.Count();
                }
                return View(await PaginatedList<PaymentReceiptVM>.CreateAsync(paymentReceiptVMs, page, pageSize));
            }
            ViewBag.count = paymentReceiptVMs.Count();
            if (pageSize == 5001)
            {
                pageSize = paymentReceiptVMs.Count();
            }
            return View(await PaginatedList<PaymentReceiptVM>.CreateAsync(paymentReceiptVMs, page, pageSize));

        }


        public async Task<IActionResult> ProfessionalIndex(string usrtext, string sortRoll, int page, int pagesize, DateTime? fromdate, DateTime? todate)
        {
            ViewBag.action = "ProfessionalIndex";
            ViewBag.controller = "Payments";

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
            }

            IQueryable<PaymentTransaction> paymentTransactions = _paymentTransactionManager.GetIQueryableData().Where(p => p.StudentCategory == 1);


           

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
        
        public async Task<IActionResult> mastersIndex(string usrtext, string sortRoll, int page, int pagesize, DateTime? fromdate, DateTime? todate)
        {
            ViewBag.action = "ProfessionalIndex";
            ViewBag.controller = "Payments";

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
            }

            IQueryable<PaymentTransaction> paymentTransactions = _paymentTransactionManager.GetIQueryableData().Where(p => p.StudentCategory == 2);


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


        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var allMeritStudents = await _meritStudentManager.GetAllAsync();

            var paymentTransaction = await _paymentTransactionManager.GetByIdAsync((int)id);
            var existMeritStudent = await _meritStudentManager.GetByAdmissionRollAsync(paymentTransaction.ReferenceNo);
            var appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(paymentTransaction.ReferenceNo);
            var existSubject = await _subjectManager.GetByCodeAsync(existMeritStudent.SubjectCode);
            var student = await _studentManager.GetByAdmissionRollAsync(existMeritStudent.NUAdmissionRoll);

            PaymentReceiptVM paymentReceiptVM = new PaymentReceiptVM();
            paymentReceiptVM.PaymentTransaction = paymentTransaction;
            paymentReceiptVM.MeritStudent = existMeritStudent;
            paymentReceiptVM.AppliedStudent = appliedStudent;
            paymentReceiptVM.Subject = existSubject;
            paymentReceiptVM.Student = student;

            TempData["msg"] = "Student Not Found";

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
                var meritStudent =await _meritStudentManager.GetByAdmissionRollAsync(paymentTransaction.ReferenceNo);
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
