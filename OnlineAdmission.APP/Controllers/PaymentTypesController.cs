using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    public class PaymentTypesController : Controller
    {
        private readonly IPaymentTypeManager _paymentTypeManager;

        public PaymentTypesController(IPaymentTypeManager paymentTypeManager)
        {
            _paymentTypeManager = paymentTypeManager;
        }

        public async Task<ActionResult> Index()
        {
            var paymentTypes = await _paymentTypeManager.GetAllAsync();
            return View(paymentTypes);
        }

        // GET: PaymentTypesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PaymentTypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PaymentTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PaymentType  model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CreatedAt = DateTime.Now;
                    model.CreatedBy = HttpContext.Session.GetString("UserId");
                    bool isSaved = await _paymentTypeManager.AddAsync(model);
                    if (isSaved)
                    {
                        return RedirectToAction(nameof(Index));

                    }
                    ViewBag.msg = "Not Created";
                }
                catch
                {
                    return View();
                }
            }
            return View();

        }

        // GET: PaymentTypesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PaymentTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: PaymentTypesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PaymentTypesController/Delete/5
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
    }
}
