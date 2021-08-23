using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineAdmission.APP.Models;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _host;
        private readonly IPaymentTransactionManager _paymentTransactionManager;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment host, IPaymentTransactionManager paymentTransactionManager)
        {
            _logger = logger;
            _host = host;
            _paymentTransactionManager = paymentTransactionManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetTransaction()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetTransaction(IFormFile transactionHistory)
        {
            string fileName = $"{ _host.WebRootPath}\\FIleData\\{ transactionHistory.FileName}";
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                transactionHistory.CopyTo(fileStream);
                fileStream.Flush();
            }
            List<PaymentTransaction> transactions =await this.GetTransactionList(fileName);

            return RedirectToAction("showData");
        }
        public async Task<IActionResult> showData()
        {
            var transactions =await _paymentTransactionManager.GetAllAsync();
            return View(transactions);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private async Task<List<PaymentTransaction>> GetTransactionList(string fName)
        {
            List<PaymentTransaction> transactions = new List<PaymentTransaction>();
            var fileName = fName; // $"{Directory.GetCurrentDirectory()}{@"\wwwroot\FileData\"}" + fName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        transactions.Add(new PaymentTransaction()
                        {
                            Amount = Convert.ToDouble(reader.GetValue(0).ToString()),
                            TransactionDate = Convert.ToDateTime(reader.GetValue(1).ToString()),
                            Balance = Convert.ToDouble(reader.GetValue(2).ToString()),
                            AccountNo = reader.GetValue(3).ToString(),
                            TransactionId = reader.GetValue(4).ToString(),
                            ReferenceNo = Convert.ToInt32(reader.GetValue(5).ToString())
                        });
                    }
                }
            }
            await _paymentTransactionManager.GetTransaction(transactions);
            return transactions;
        }
    }
}
