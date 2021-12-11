using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.BLL.IManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    public class StudentPaymentTypesController : Controller
    {
        private readonly IStudentPaymentTypeManager _paymentTypeManager;

        public StudentPaymentTypesController(IStudentPaymentTypeManager paymentTypeManager)
        {
            _paymentTypeManager = paymentTypeManager;
        }
        public ActionResult Index()
        {
            return View();
        }

        // GET: StudentPaymentTypesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: StudentPaymentTypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StudentPaymentTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: StudentPaymentTypesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: StudentPaymentTypesController/Edit/5
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

        // GET: StudentPaymentTypesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StudentPaymentTypesController/Delete/5
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
