﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class PaymentsController : Controller
    {
        private readonly IPaymentTransactionManager _paymentTransactionManager;
        private readonly IStudentManager _studentManager;
        private readonly ISubjectManager _subjectManager;

        public PaymentsController(IPaymentTransactionManager paymentTransactionManager, IStudentManager studentManager, ISubjectManager subjectManager)
        {
            _paymentTransactionManager = paymentTransactionManager;
            _studentManager = studentManager;
            _subjectManager = subjectManager;
        }

        // GET: Payments
        public async Task<IActionResult> Index(string searchingText, string sortRoll, string sortHSCRoll, int page, int pagesize)
        {

            IQueryable<PaymentTransaction> transactions = _paymentTransactionManager.GetIQueryableData();

            ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : " ";


            switch (sortRoll)
            {
                case "desc":
                    transactions = transactions.OrderByDescending(m => m.ReferenceNo);
                    break;
                default:
                    transactions = transactions.OrderBy(m => m.ReferenceNo);
                    break;
            }


            ViewBag.searchingText = searchingText;
            ViewBag.count = transactions.Count();

            int pageSize = pagesize <= 0 ? 10 : pagesize;
            if (page <= 0) page = 1;

            if (!string.IsNullOrEmpty(searchingText))
            {
                searchingText = searchingText.Trim().ToLower();

                transactions = transactions.Where(m => m.ReferenceNo.ToString().ToLower() == searchingText || m.TransactionDate.ToString().ToLower() == searchingText || m.Amount.ToString().ToLower() == searchingText || m.AdmissionFee.ToString().ToLower() == searchingText);

                return View(await PaginatedList<PaymentTransaction>.CreateAsync(transactions, page, pageSize));
            }

            return View(await PaginatedList<PaymentTransaction>.CreateAsync(transactions, page, pageSize));

        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var allStudents = await _studentManager.GetAllAsync();
            var paymentTransaction = await _paymentTransactionManager.GetByIdAsync((int)id);



            var existStudent = allStudents.Where(s => s.NUAdmissionRoll == paymentTransaction.ReferenceNo).FirstOrDefault();
            //var paymentTransactions = await _paymentTransactionManager.GetAllAsync();
            //var selectedTransactions = paymentTransactions.Where(t => t.ReferenceNo == existStudent.NUAdmissionRoll).ToList();
            var subject = await _subjectManager.GetByIdAsync(existStudent.SubjectId);
            PaymentReceiptVM paymentReceiptVM = new PaymentReceiptVM();
            paymentReceiptVM.PaymentTransaction = paymentTransaction;
            paymentReceiptVM.Student = existStudent;
            paymentReceiptVM.Subject = subject;
            return View(paymentReceiptVM);








            

            //if (paymentTransaction == null)
            //{
            //    return NotFound();
            //}

            //return View(paymentTransaction);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,TransactionDate,Balance,AccountNo,TransactionId,ReferenceNo,AdmissionFee,ServiceCharge")] PaymentTransaction paymentTransaction)
        {
            if (ModelState.IsValid)
            {
                await _paymentTransactionManager.AddAsync(paymentTransaction);
                return RedirectToAction(nameof(Index));
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
