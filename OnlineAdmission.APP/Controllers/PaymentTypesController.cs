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
        public async Task<ActionResult> Edit(int id)
        {
            var paymentTypes = await _paymentTypeManager.GetByIdAsync(id);

            return View(paymentTypes);
        }

        // POST: PaymentTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, PaymentType model)
        {
            //try
            //{
            //    return RedirectToAction(nameof(Index));
            //}
            //catch
            //{
            //    return View();
            //}
            if (id != model.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //var paymentTypeList = await _paymentTypeManager.GetAllAsync();
                //var paymentTypeExist = paymentTypeList.FirstOrDefault(p => p.PaymentTypeName==model.PaymentTypeName && p.Id != id);
                //if (paymentTypeExist == null)
                //{
                    try
                    {
                        model.UpdatedAt = DateTime.Now;
                        model.UpdatedBy = HttpContext.Session.GetString("UserId");
                        //bool isSaved = await _paymentTypeManager.UpdateAsync(paymentType);
                        //if (isSaved)
                        //{
                        //    return RedirectToAction(nameof(Index));
                        //}
                        //ViewBag.msg = "Not updated";

                        //return View(paymentType);
                        await _paymentTypeManager.UpdateAsync(model);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        return View(ex);
                    }
                //}
                //ViewBag.msg = "This session is already exist.";
            }
            return View(model);
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
