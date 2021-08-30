using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    public class SMSController : Controller
    {
        // GET: SMSController
        public ActionResult Index()
        {
            return View();
        }

        // GET: SMSController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SMSController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SMSController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SMSModel model)
        {
            if (ModelState.IsValid)
            {
                return View();
            }
            return View();
        }

        // GET: SMSController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SMSController/Edit/5
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

        // GET: SMSController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SMSController/Delete/5
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
