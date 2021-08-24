using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.API.ViewModels;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineAdmission.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTransactionsController : ControllerBase
    {
        private readonly IPaymentTransactionManager _paymentTransactionManager;
        private readonly IMeritStudentManager _meritStudentManager;
        public PaymentTransactionsController(IPaymentTransactionManager paymentTransactionManager, IMeritStudentManager meritStudentManager)
        {
            _paymentTransactionManager = paymentTransactionManager;
            _meritStudentManager = meritStudentManager;
        }
        // GET: api/<PaymentTransactionsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<PaymentTransactionsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PaymentTransactionsController>
        [HttpPost("add-transaction")]
        public async Task<IActionResult> Add([FromBody] TransactionInfo model)
        {
            if (model.Status.ToLower()=="success")
            {
                model.Notification = "Payment successfull";

                PaymentTransaction newPayment = new PaymentTransaction();

                //newPayment.Amount = model.PaymentTransaction.Amount;
                //newPayment.TransactionDate = DateTime.Today;
                //newPayment.Balance = model.PaymentTransaction.Balance;
                //newPayment.AccountNo = model.PaymentTransaction.AccountNo;
                //var guid = Guid.NewGuid();
                //newPayment.TransactionId = guid.ToString();
                //newPayment.ReferenceNo = model.PaymentTransaction.ReferenceNo;

                //await _paymentTransactionManager.AddAsync(newPayment);

                //MeritStudent meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(model.NuRoll);
                //meritStudent.PaymentStatus = true;
                //meritStudent.PaymentTransactionId = newPayment.Id;
                //await _meritStudentManager.UpdateAsync(meritStudent);
                //string site = "https://localhost:44356/";
                string site = "http://115.127.26.3:4356/";
                string param = "students/search?notification="+model.Notification;
                return Redirect(site+param);
            }

            return Ok();
        
        }
        

        // PUT api/<PaymentTransactionsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PaymentTransactionsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
