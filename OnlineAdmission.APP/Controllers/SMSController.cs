using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class SMSController : Controller
    {
        private readonly ISMSManager _smsManager;

        public SMSController(ISMSManager smsManager)
        {
            _smsManager = smsManager;
        }
        // GET: SMSController
        public async Task<ActionResult> Index()
        {
            var sms = await _smsManager.GetAllAsync();
            return View(sms);
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
