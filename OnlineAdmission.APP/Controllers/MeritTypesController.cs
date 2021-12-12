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
    public class MeritTypesController : Controller
    {
        private readonly IMeritTypeManager _meritTypeManager;

        public MeritTypesController(IMeritTypeManager meritTypeManager)
        {
            _meritTypeManager = meritTypeManager;
        }

        public async Task<ActionResult> Index()
        {
            var meritTypes = await _meritTypeManager.GetAllAsync();
            return View(meritTypes);
        }

        // GET: MeritTypesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MeritTypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MeritTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MeritType model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CreatedAt = DateTime.Now;
                    model.CreatedBy = HttpContext.Session.GetString("UserId");
                    bool isSaved = await _meritTypeManager.AddAsync(model);
                    if (isSaved)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    ViewBag.msg = "not created";
                }
                catch
                {
                    return View();
                }
            }
            return View();
        }

        // GET: MeritTypesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MeritTypesController/Edit/5
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

        // GET: MeritTypesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MeritTypesController/Delete/5
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
