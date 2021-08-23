using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineAdmission.APP.ViewModels;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    public class SpecialOfferController : Controller
    {
        private readonly IMeritStudentManager _meritStudentManager;
        private readonly IAppliedStudentManager _appliedStudentManager;
        private readonly ISubjectManager _subjectManager;

        public SpecialOfferController(IMeritStudentManager meritStudentManager, IAppliedStudentManager appliedStudentManager, ISubjectManager subjectManager)
        {
            _meritStudentManager = meritStudentManager;
            _appliedStudentManager = appliedStudentManager;
            _subjectManager = subjectManager;
        }
        // GET: SpecialOfferController
        public async Task<ActionResult> Index()
        {
            var specialStudent = await _meritStudentManager.GetSpecialPaymentStudent();
            List<SpecialOfferVM> specialOfferVMList = new List<SpecialOfferVM>();
            SpecialOfferVM specialOfferVM = new SpecialOfferVM();
            foreach (var item in specialStudent)
            {
                var appliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(item.NUAdmissionRoll);
                var subject = await _subjectManager.GetByCodeAsync(item.SubjectCode);
                specialOfferVM.AppliedStudent = appliedStudent;
                specialOfferVM.MeritStudent = item;
                specialOfferVM.Subject = subject;

                specialOfferVMList.Add(specialOfferVM);
            }

            return View(specialOfferVMList);
        }

        // GET: SpecialOfferController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SpecialOfferController/Create
        public async Task<ActionResult> Create()
        {
            List<SpecialOfferVM> specialOfferVMList = new List<SpecialOfferVM>();
            SpecialOfferVM specialOfferVM = new SpecialOfferVM();

            var meritStudents = await _meritStudentManager.GetAllWithoutPaidAsync();

            foreach (var item in meritStudents)
            {

                MeritStudent newMerit = new MeritStudent();
                newMerit = item;
                AppliedStudent newApplied = new AppliedStudent();
                newApplied = await _appliedStudentManager.GetByAdmissionRollAsync(item.NUAdmissionRoll);

                specialOfferVM.MeritStudent = newMerit;
                specialOfferVM.AppliedStudent = newApplied;

                specialOfferVMList.Add(specialOfferVM);
            }
            
            ViewData["meritStudentId"] = new SelectList((from s in specialOfferVMList.ToList()
                                                     select new
                                                     {
                                                         Id = s.MeritStudent.Id,
                                                         FullName = s.AppliedStudent.ApplicantName +"("+ s.MeritStudent.NUAdmissionRoll+")"
                                                     }), "Id", "FullName", null);

            return View();
        }


        // POST: SpecialOfferController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SpecialOfferVM model)
        {
            if (ModelState.IsValid)
            {
                MeritStudent meritStudent = await _meritStudentManager.GetByIdAsync(model.MeritStudentId);
                meritStudent.DeductedAmaount = model.Amount;
                await _meritStudentManager.UpdateAsync(meritStudent);
                return View();
            }

            SpecialOfferVM spO = new SpecialOfferVM();
            spO.MeritStudentList = new SelectList(await _appliedStudentManager.GetAllAsync(), "Id", "ApplicantName").ToList();
            return View(spO);
        }

        // GET: SpecialOfferController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SpecialOfferController/Edit/5
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

        // GET: SpecialOfferController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SpecialOfferController/Delete/5
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
