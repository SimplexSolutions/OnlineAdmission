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
    [Authorize(Roles ="Admin, SuperAdmin")]
    public class MeritStudentsController : Controller
    {
        private readonly IMeritStudentManager _meritStudentManager;
        private readonly IWebHostEnvironment _host;
        private readonly IAppliedStudentManager _appliedStudentManager;
        private readonly IStudentCategoryManager _studentCategoryManager;

        public MeritStudentsController(IMeritStudentManager meritStudentManager, IWebHostEnvironment host, IAppliedStudentManager appliedStudentManager, IStudentCategoryManager studentCategoryManager)
        {
            _meritStudentManager = meritStudentManager;
            _host = host;
            _appliedStudentManager = appliedStudentManager;
            _studentCategoryManager = studentCategoryManager;
        }

        // GET: MeritStudentsController
        public async Task<ActionResult> Index(string usrtext, string sortRoll, int page, int pagesize, int? studentCategory)
        {

            ViewBag.action = "Index";
            ViewBag.controller = "MeritStudents";
            if (TempData["duplicateCount"]!=null)
            {
                ViewBag.duplicateCount = TempData["duplicateCount"].ToString();
            }
            if (TempData["savedCount"] != null)
            {
                ViewBag.savedCount = TempData["savedCount"].ToString();
            }
            IQueryable<MeritStudent> meritStudentList = _meritStudentManager.GetMeritStudents();
            var studentCategoryFromSession = HttpContext.Session.GetString("studentCategoryMerit");

            if (studentCategory!=null && studentCategory > 0)
            {
                ViewBag.studentCategory = studentCategory;
                HttpContext.Session.SetString("studentCategoryMerit", studentCategory.ToString());
                meritStudentList = _meritStudentManager.GetMeritStudentsByCategory((int)studentCategory);
            }
            
            else if (!string.IsNullOrEmpty(studentCategoryFromSession))
            {
                ViewBag.studentCategory = studentCategoryFromSession;
                meritStudentList = _meritStudentManager.GetMeritStudentsByCategory(Convert.ToInt32(studentCategoryFromSession));
            }
            else
            {
                ViewBag.studentCategory = "";
                HttpContext.Session.SetString("studentCategoryMerit", "");
            }

            if (studentCategory == 0)
            {
                meritStudentList = _meritStudentManager.GetMeritStudents();
                ViewBag.studentCategory = "";
                HttpContext.Session.SetString("studentCategoryMerit", "");
            }
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

            
            ViewBag.data = usrtext;
            ViewBag.count = meritStudentList.Count();
            
            int pageSize = pagesize <= 0 ? 20 : pagesize;
            if (page <= 0) page = 1;
            ViewBag.StudentCategoryList = new SelectList(await _studentCategoryManager.GetAllAsync(), "Id", "CategoryName");
            if (!string.IsNullOrEmpty(usrtext))
            {
                usrtext = usrtext.Trim().ToLower();

                meritStudentList = meritStudentList.Where(m => m.NUAdmissionRoll.ToString() == usrtext || m.HSCRoll.ToString() == usrtext || m.MeritPosition.ToString() == usrtext || m.SubjectCode.ToString() == usrtext || m.Comments.ToLower().Trim().Contains(usrtext));

                ViewBag.count = meritStudentList.Count();
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

            //collection.UpdatedBy = HttpContext.Session.GetString("UserId");
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
            if (stuList!=null)
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
            else
            {
                ViewBag.msg = "Please select a file before upload.";
                return View();
            }
            
        }


        private async Task<List<MeritStudent>> GetStudentsList(string fName)
        {
            List<MeritStudent> students = new List<MeritStudent>(); 
            int duplicateCount = 0;
            int savedItem = 0;
            var fileName = fName; // $"{Directory.GetCurrentDirectory()}{@"\wwwroot\FileData\"}" + fName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
               
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    
                    while (reader.Read())
                    {
                        var existingMeritList = await _meritStudentManager.GetByAdmissionRollAsync(Convert.ToInt32(reader.GetValue(0).ToString()), Convert.ToInt32(reader.GetValue(5).ToString()), reader.GetValue(4).ToString());
                        if (existingMeritList != null)
                        {
                            duplicateCount++;
                            continue;
                        }
                        savedItem++;
                        students.Add(new MeritStudent()
                        {
                            NUAdmissionRoll = Convert.ToInt32(reader.GetValue(0).ToString()),
                            HSCRoll = Convert.ToInt64(reader.GetValue(1).ToString()),
                            MeritPosition = Convert.ToDouble(reader.GetValue(2).ToString()),
                            SubjectCode = Convert.ToInt32(reader.GetValue(3).ToString()),
                            Comments = reader.GetValue(4).ToString(),
                            StudentCategoryId = Convert.ToInt32(reader.GetValue(5).ToString()),
                            HonorsRoll = Convert.ToInt64(reader.GetValue(6).ToString()),
                            AcademicSessionId = Convert.ToInt32(reader.GetValue(7).ToString()),
                            MeritTypeId = Convert.ToInt32(reader.GetValue(8).ToString())
                        });

                    }
                }
            }
            await _meritStudentManager.UploadMeritStudentsAsync(students);
            TempData["duplicateCount"] = duplicateCount;
            TempData["savedCount"] = savedItem;
            return students;
        }

        
    }
}
