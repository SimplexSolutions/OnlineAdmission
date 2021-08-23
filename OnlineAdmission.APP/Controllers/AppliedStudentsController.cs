using AutoMapper;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.APP.ViewModels.AppliedStudents;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    [Authorize]
    public class AppliedStudentsController : Controller
    {
        private readonly IAppliedStudentManager _appliedStudentManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _host;

        public AppliedStudentsController(IAppliedStudentManager appliedStudentManager, IMapper mapper, IWebHostEnvironment host)
        {
            _appliedStudentManager = appliedStudentManager;
            _mapper = mapper;
            _host = host;
        }
        public async Task<IActionResult> Index()
        {
            var appliedStudentList = await _appliedStudentManager.GetAllAsync();
            return View(appliedStudentList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppliedStudentVM vModel)
        {
            if (ModelState.IsValid)
            {
                AppliedStudent meritStudent = _mapper.Map<AppliedStudent>(vModel);

                await _appliedStudentManager.AddAsync(meritStudent);
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpGet]
        public IActionResult GetAppliedStudentList()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAppliedStudentList(IFormFile stuList)
        {
            if (stuList == null)
            {
                ViewBag.msg = "Pleas Select a file";
                return View();
            }
            string fileName = $"{ _host.WebRootPath}\\FIleData\\{ stuList.FileName}";
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                stuList.CopyTo(fileStream);
                fileStream.Flush();
            }
            //List<StInfoTable> students = this.GetStudentsList(file.FileName);
            List<AppliedStudent> students = await this.GetStudentsList(fileName);

            return RedirectToAction("Index");
        }

        private async Task<List<AppliedStudent>> GetStudentsList(string fName)
        {
            List<AppliedStudent> students = new List<AppliedStudent>();
            var fileName = fName; // $"{Directory.GetCurrentDirectory()}{@"\wwwroot\FileData\"}" + fName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        students.Add(new AppliedStudent()
                        {
                            NUAdmissionRoll = Convert.ToInt32(reader.GetValue(0).ToString()),
                            ApplicantName = reader.GetValue(1).ToString(),
                            FatherName = reader.GetValue(2).ToString(),
                            MotherName = reader.GetValue(3).ToString(),
                            MobileNo = reader.GetValue(4).ToString(),
                            HSCGroup = reader.GetValue(5).ToString()
                        });
                    }
                }
            }
            await _appliedStudentManager.UploadAppliedStudentsAsync(students);
            return students;
        }

    }
}
