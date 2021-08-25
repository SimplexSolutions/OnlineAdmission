﻿using Microsoft.AspNetCore.Http;
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
            
            foreach (var item in specialStudent)
            {
                SpecialOfferVM specialOfferVM = new SpecialOfferVM();
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
            

            var meritStudents = await _meritStudentManager.GetAllWithoutPaidAsync();

            foreach (var item in meritStudents)
            {
                SpecialOfferVM specialOfferVM = new SpecialOfferVM();
                specialOfferVM.AppliedStudent = await _appliedStudentManager.GetByAdmissionRollAsync(item.NUAdmissionRoll);
                specialOfferVM.Subject = await _subjectManager.GetByCodeAsync(item.SubjectCode);
                specialOfferVM.MeritStudent = item;
                specialOfferVMList.Add(specialOfferVM);

            }
            
            ViewData["meritStudentId"] = new SelectList((from s in specialOfferVMList
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
                MeritStudent meritStudent = await _meritStudentManager.GetByIdAsync(model.MeritStudent.Id);
                meritStudent.DeductedAmaount = model.Amount;
                await _meritStudentManager.UpdateAsync(meritStudent);
                return RedirectToAction("Index");
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
