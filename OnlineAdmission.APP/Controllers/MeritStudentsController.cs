using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineAdmission.APP.ViewModels;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OnlineAdmission.APP.Controllers
{
    [Authorize]
    public class MeritStudentsController : Controller
    {
        private readonly IMeritStudentManager _meritStudentManager;
        private readonly IWebHostEnvironment _host;
        private readonly IAppliedStudentManager _appliedStudentManager;

        public MeritStudentsController(IMeritStudentManager meritStudentManager, IWebHostEnvironment host, IAppliedStudentManager appliedStudentManager)
        {
            _meritStudentManager = meritStudentManager;
            _host = host;
            _appliedStudentManager = appliedStudentManager;
        }

        // GET: MeritStudentsController
        public async Task<ActionResult> Index(string searchingText, string sortRoll, string sortHSCRoll, int page, int pagesize)
        {

            IQueryable<MeritStudent> meritStudentList = _meritStudentManager.GetMeritStudents();

            ViewBag.sortByRoll = string.IsNullOrEmpty(sortRoll) ? "desc" : " ";

            
            switch (sortRoll)
            {
                case "desc":
                    meritStudentList = meritStudentList.OrderByDescending(m => m.NUAdmissionRoll);
                    break;
                default:
                    meritStudentList = meritStudentList.OrderBy(m => m.NUAdmissionRoll);
                    break;
            }

            
            ViewBag.searchingText = searchingText;
            ViewBag.count = meritStudentList.Count();
            
            int pageSize = pagesize <= 0 ? 10 : pagesize;
            if (page <= 0) page = 1;

            if (!string.IsNullOrEmpty(searchingText))
            {
                searchingText = searchingText.Trim().ToLower();

                meritStudentList = meritStudentList.Where(m => m.NUAdmissionRoll.ToString().ToLower() == searchingText || m.HSCRoll.ToString().ToLower() == searchingText || m.MeritPosition.ToString().ToLower() == searchingText || m.SubjectCode.ToString().ToLower() == searchingText);

                return View(await PaginatedList<MeritStudent>.CreateAsync(meritStudentList, page, pageSize));
            }

            return View(await PaginatedList<MeritStudent>.CreateAsync(meritStudentList, page, pageSize));
        }

        // GET: MeritStudentsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var meritStudent =await _meritStudentManager.GetByIdAsync(id);
            return View(meritStudent);
        }

        // GET: MeritStudentsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MeritStudentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MeritStudent model)
        {
            if (ModelState.IsValid)
            {
                await _meritStudentManager.AddAsync(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        

        // GET: MeritStudentsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var meritStudent = await _meritStudentManager.GetByIdAsync(id);
            return View(meritStudent);

        }

        // POST: MeritStudentsController/Edit/5
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

        // GET: MeritStudentsController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var meritStudent = await _meritStudentManager.GetByIdAsync(id);
            return View(meritStudent);
        }

        // POST: MeritStudentsController/Delete/5
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

        [HttpGet]
        public IActionResult GetMeritStudentList()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetMeritStudentList(IFormFile stuList)
        {
            string fileName = $"{ _host.WebRootPath}\\FIleData\\{ stuList.FileName}";
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                stuList.CopyTo(fileStream);
                fileStream.Flush();
            }
            //List<StInfoTable> students = this.GetStudentsList(file.FileName);
            List<MeritStudent> students =await this.GetStudentsList(fileName);

            return RedirectToAction("Index");
        }


        private async Task<List<MeritStudent>> GetStudentsList(string fName)
        {
            List<MeritStudent> students = new List<MeritStudent>();
            var fileName = fName; // $"{Directory.GetCurrentDirectory()}{@"\wwwroot\FileData\"}" + fName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        students.Add(new MeritStudent()
                        {
                            NUAdmissionRoll = Convert.ToInt32(reader.GetValue(0).ToString()),
                            HSCRoll = Convert.ToInt32(reader.GetValue(1).ToString()),
                            MeritPosition = Convert.ToInt32(reader.GetValue(2).ToString()),
                            SubjectCode = Convert.ToInt32(reader.GetValue(3).ToString()),
                            PaymentStatus = Convert.ToBoolean(reader.GetValue(4).ToString()),
                            //PaidAmaount = Convert.ToDouble(reader.GetValue(5).ToString()),
                            Comments = reader.GetValue(6).ToString()
                        });

                    }
                }
            }
            await _meritStudentManager.UploadMeritStudentsAsync(students);
            return students;
        }

        
    }
}
