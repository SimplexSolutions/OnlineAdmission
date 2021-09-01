using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.DB;
using OnlineAdmission.Entity;

namespace OnlineAdmission.APP.Controllers
{
    [Authorize(Roles ="Admin,SuperAdmin")]
    public class SubjectsController : Controller
    {
        private readonly ISubjectManager _subjectManager;

        public SubjectsController(ISubjectManager subjectManager)
        {
            _subjectManager = subjectManager;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            return View(await _subjectManager.GetAllAsync());
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _subjectManager.GetByIdAsync((int)id);

            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubjectName,Code,AdmissionFee")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                await _subjectManager.AddAsync(subject);
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _subjectManager.GetByIdAsync((int)id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectName,Code,AdmissionFee")] Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _subjectManager.UpdateAsync(subject);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await SubjectExists(subject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _subjectManager.GetByIdAsync((int)id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _subjectManager.GetByIdAsync((int)id);
            await _subjectManager.RemoveAsync(subject);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SubjectExists(int id)
        {
            var subject = await _subjectManager.GetByIdAsync((int)id);
            if (subject!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
