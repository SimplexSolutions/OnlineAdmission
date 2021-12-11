using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace OnlineAdmission.APP.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class AcademicSessionsController : Controller
    {
        private readonly IAcademicSessionManager _academicSessionManager;

        public AcademicSessionsController(IAcademicSessionManager academicSessionManager)
        {
            _academicSessionManager = academicSessionManager;
        }

        public async Task<ActionResult> Index()
        {
            var sessions = await _academicSessionManager.GetAllAsync();
            return View(sessions);
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
        public async Task<ActionResult> Create(AcademicSession model)
        {
            if (ModelState.IsValid)
            {
                var sessions = await _academicSessionManager.GetAllAsync();
                bool isExist = sessions.FirstOrDefault(s => s.SessionName.Trim() == model.SessionName.Trim()) != null;
                if (isExist==true)
                {
                    ViewBag.msg = "This session is already exist.";
                    return View(model);
                }
                try
                {
                    model.CreatedAt = DateTime.Now;
                    model.CreatedBy = HttpContext.Session.GetString("UserId");
                    await _academicSessionManager.AddAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            return View();
        }

        // GET: AcademicSessionsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var aSession = await _academicSessionManager.GetByIdAsync(id);
            if (aSession == null)
            {
                TempData["msg"] = "Session is available";
                return RedirectToAction("Index");
            }

            return View(aSession);
        }

        // POST: AcademicSessionsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AcademicSession academicSession)
        {
            if (id != academicSession.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var sessionList = await _academicSessionManager.GetAllAsync();
                var sessionExist = sessionList.FirstOrDefault(s => s.SessionName == academicSession.SessionName && s.Id != id);
                if (sessionExist == null)
                {
                    try
                    {
                        academicSession.UpdatedAt = DateTime.Now;
                        academicSession.UpdatedBy = HttpContext.Session.GetString("UserId");
                        bool isSaved = await _academicSessionManager.AddAsync(academicSession);
                        if (isSaved)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        ViewBag.msg = "Not updated";
                        return View(academicSession);
                    }
                    catch
                    {
                        return View();
                    }
                }
                ViewBag.msg = "This session is already exist.";
            }
            return View(academicSession);
            
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
