using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.BLL.IManager;
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

        public ActionResult Index()
        {
            return View();
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
