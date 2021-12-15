using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin,SuperAdmin")]        
    public class StudentCategoriesController : Controller
    {
        private readonly IStudentCategoryManager _studentCategoryManager;
        private readonly IAcademicSessionManager _academicSessionManager;

        public StudentCategoriesController(IStudentCategoryManager studentCategoryManager, IAcademicSessionManager academicSessionManager)
        {
            _studentCategoryManager = studentCategoryManager;
            _academicSessionManager = academicSessionManager;
        }
        public async Task<IActionResult> Index()
        {
            var stCat = await _studentCategoryManager.GetAllAsync();
            return View(stCat);
        }

        public IActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentCategory model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                model.CreatedBy = HttpContext.Session.GetString("UserId");
                await _studentCategoryManager.AddAsync(model);
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cat = await _studentCategoryManager.GetByIdAsync(id);
            
            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, StudentCategory model)
        {

            if (model.Id!=id)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                model.UpdatedAt = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("UserId");

                await _studentCategoryManager.UpdateAsync(model);
                return RedirectToAction("Index");
            }

            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _studentCategoryManager.GetByIdAsync(id);
            return View(cat);
        }
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var cat = await _studentCategoryManager.GetByIdAsync(id);
            if (cat!=null)
            {
                await _studentCategoryManager.RemoveAsync(cat);
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}
