using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.API.ViewModels;
using OnlineAdmission.API.ViewModels.MeritStudent;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeritStudentsController : ControllerBase
    {
        private readonly IMeritStudentManager _meritStudentManager;
        private readonly IMapper _mapper;
        private readonly IPaymentTransactionManager _paymentTransactionManager;
        public MeritStudentsController(IMeritStudentManager meritStudentManager, IMapper mapper, IPaymentTransactionManager paymentTransactionManager)
        {
            _meritStudentManager = meritStudentManager;
            _mapper = mapper;
            _paymentTransactionManager = paymentTransactionManager;
        }


        [HttpGet("get-all-student")]
        public async Task<ActionResult<IEnumerable<MeritStudent>>> GetStudents()
        {
            var stuList = await _meritStudentManager.GetAllAsync();
            return stuList.ToList();
        }

        [HttpGet("get-student-by-id/{id}")]
        public async Task<ActionResult<MeritStudent>> GetStudentById(int id)
        {
            var stu =await  _meritStudentManager.GetByIdAsync(id);
            return stu;
        }


        [HttpPost("add-student")]
        public async Task<ActionResult<MeritStudent>> AddMeritStudent([FromBody] MeritStudentCreateVM newMeritStudent)
        {
            var mStudent = _mapper.Map<MeritStudent>(newMeritStudent);
            await _meritStudentManager.AddAsync(mStudent);
            return mStudent;
        }

        [HttpPut("Update-student-by-id/{id}")]
        public async Task<ActionResult<MeritStudent>> UpdatedById(int id, [FromBody] MeritStudentEditVM existingMeritStudent)
        {
            var mStudent = _mapper.Map<MeritStudent>(existingMeritStudent);
            
            if (id != mStudent.Id)
            {
                return BadRequest();
            }
            await _meritStudentManager.UpdateAsync(mStudent);
            return mStudent;
        }

        [HttpPut("Update-From-Nagad")]
        public async Task<ActionResult<string>> NagadPayment([FromBody]TransactionInfo model)
        {
            PaymentTransaction newPayment = new PaymentTransaction();

            newPayment.Amount = model.PaymentTransaction.Amount;
            newPayment.TransactionDate = DateTime.Today;
            newPayment.Balance = 0000;
            newPayment.AccountNo = model.PaymentTransaction.AccountNo;
            var guid = Guid.NewGuid();
            newPayment.TransactionId = guid.ToString();
            newPayment.ReferenceNo = model.PaymentTransaction.ReferenceNo;

            await _paymentTransactionManager.AddAsync(newPayment);


            MeritStudent meritStudent = await _meritStudentManager.GetByAdmissionRollAsync(model.PaymentTransaction.ReferenceNo);
            if (meritStudent != null)
            {
                meritStudent.PaymentStatus = true;
                PaymentTransaction paymentTransaction = await _paymentTransactionManager.GetTransactionByNuRollAsync(model.NuRoll);
                if (paymentTransaction != null)
                {
                    meritStudent.PaymentTransactionId = paymentTransaction.Id;
                    await _meritStudentManager.UpdateAsync(meritStudent);
                    return RedirectToAction("Search", new { nuAdmissionRoll = model.PaymentTransaction.ReferenceNo });
                }
            }

            return "success";

        }
    }
}
