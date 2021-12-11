using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.BLL.IManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    public class AcademicSessionsController : Controller
    {
        private readonly IAcademicSessionManager _academicSessionManager;

        public AcademicSessionsController(IAcademicSessionManager academicSessionManager)
        {
            _academicSessionManager = academicSessionManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET: AcademicSessionsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AcademicSessionsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AcademicSessionsController/Create
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

        // GET: AcademicSessionsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AcademicSessionsController/Edit/5
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

        // GET: AcademicSessionsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AcademicSessionsController/Delete/5
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
