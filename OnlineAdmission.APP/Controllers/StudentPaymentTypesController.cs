using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    public class StudentPaymentTypesController : Controller
    {
        private readonly IStudentPaymentTypeManager _studentPaymentTypeManager;
        private readonly IStudentCategoryManager _studentCategoryManager;
        private readonly IMeritTypeManager _meritTypeManager;
        private readonly IPaymentTypeManager _paymentTypeManager;
        private readonly IAcademicSessionManager _academicSessionManager;
        public StudentPaymentTypesController(IStudentPaymentTypeManager studentPaymentTypeManager, IAcademicSessionManager academicSessionManager, IPaymentTypeManager paymentTypeManager, IMeritTypeManager meritTypeManager, IStudentCategoryManager studentCategoryManager)
        {
            _studentPaymentTypeManager = studentPaymentTypeManager;
            _meritTypeManager = meritTypeManager;
            _studentCategoryManager = studentCategoryManager;
            _paymentTypeManager = paymentTypeManager;
            _academicSessionManager = academicSessionManager;

        }
        public async Task<ActionResult> Index()
        {
            var paymentTypes = await _studentPaymentTypeManager.GetAllAsync();
            return View(paymentTypes);
        }

        // GET: StudentPaymentTypesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: StudentPaymentTypesController/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.StudentCategoryId = new SelectList(await _studentCategoryManager.GetAllAsync(), "Id", "CategoryName");
            ViewBag.MeritTypeId = new SelectList(await _meritTypeManager.GetAllAsync(), "Id", "MeritTypeName");
            ViewBag.PaymentTypeId = new SelectList(await _paymentTypeManager.GetAllAsync(), "Id", "PaymentTypeName");
            ViewBag.AcademicSessionId = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "SessionName");
            return View();
        }

        // POST: StudentPaymentTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StudentPaymentType model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CreatedAt = DateTime.Now;
                    model.CreatedBy = HttpContext.Session.GetString("UserId");
                    bool isSaved = await _studentPaymentTypeManager.AddAsync(model);
                    if (isSaved)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    ViewBag.msg = "Not created";
                }
                catch
                {

                }
            }
            

            ViewBag.StudentCategoryId = new SelectList(await _studentCategoryManager.GetAllAsync(), "Id", "CategoryName", model.StudentCategoryId);
            ViewBag.MeritTypeId = new SelectList(await _meritTypeManager.GetAllAsync(), "Id", "MeritTypeName", model.MeritTypeId);
            ViewBag.PaymentTypeId = new SelectList(await _paymentTypeManager.GetAllAsync(), "Id", "PaymentTypeName", model.PaymentTypeId);
            ViewBag.AcademicSessionId = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "SessionName", model.AcademicSessionId);

            return View(model);
        }

        // GET: StudentPaymentTypesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var model = await _studentPaymentTypeManager.GetByIdAsync(id);

            ViewBag.StudentCategoryId = new SelectList(await _studentCategoryManager.GetAllAsync(), "Id", "CategoryName", model.StudentCategoryId);
            ViewBag.MeritTypeId = new SelectList(await _meritTypeManager.GetAllAsync(), "Id", "MeritTypeName", model.MeritTypeId);
            ViewBag.PaymentTypeId = new SelectList(await _paymentTypeManager.GetAllAsync(), "Id", "PaymentTypeName", model.PaymentTypeId);
            ViewBag.AcademicSessionId = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "SessionName", model.AcademicSessionId);
            return View(model);
        }

        // POST: StudentPaymentTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, StudentPaymentType model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                model.UpdatedAt = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("UserId");

                bool isUpdated = await _studentPaymentTypeManager.UpdateAsync(model);
                if (isUpdated)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            
            return View();
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
