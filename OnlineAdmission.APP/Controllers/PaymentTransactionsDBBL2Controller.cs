using DBBL_ServiceTest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.APP.Utilities.DBBLUtilities;
using OnlineAdmission.APP.Utilities.SMS;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    public class PaymentTransactionsDBBL2Controller : Controller
    {
        private readonly IMeritStudentManager _meritStudentManager;
        private readonly IPaymentTransactionManager paymentTransactionManager;
        private readonly IAppliedStudentManager _appliedStudentManager;
        private readonly ISMSManager _smsManager;
        private IHttpContextAccessor _accessor;

        public PaymentTransactionsDBBL2Controller(IMeritStudentManager meritStudentManager, IPaymentTransactionManager paymentTransactionManager, ISMSManager sMSManager,
           IAppliedStudentManager appliedStudentManager, IHttpContextAccessor accessor)
        {
            _meritStudentManager = meritStudentManager;
            this.paymentTransactionManager = paymentTransactionManager;
            _appliedStudentManager = appliedStudentManager;
            //_studentManager = studentManager;
            _smsManager = sMSManager;
            _accessor = accessor;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTransactionResult(string trans_id)
        {
            var result = new Dictionary<string, object>();

            string _user = string.Empty;
            string _pass = string.Empty;


            _user = DBBL_Utilities.user;
            _pass = DBBL_Utilities.pass;


            string Clintip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            string Txnrefnum = "Test";

            ServicePointManager.Expect100Continue = true;

            var dbbl = new dbblecomtxnClient();
            var transResult = await dbbl.getresultfieldAsync(_user, _pass, trans_id, Clintip, Txnrefnum);
            if (transResult.Body.@return.Length > 0)
            {
                var payment = await paymentTransactionManager.GetPaymentTransactionByTrId(trans_id);
                if (payment != null)
                {
                    payment.PaymentStatus = true;
                    await paymentTransactionManager.UpdateAsync(payment);

                    MeritStudent meritStudent = new MeritStudent();
                    meritStudent = await _meritStudentManager.GetMeritStudentAsync(payment.ReferenceNo, (int)payment.StudentCategoryId, DBBL_Utilities.MeritTypeId, (int)payment.AcademicSessionId);
                    string phoneNumber;
                    string msgText;

                    phoneNumber = payment.MobileNumber;
                    msgText = "Congratulations! your payment has been successfully paid";

                    if (meritStudent != null)
                    {
                        if (payment.PaymentTypeId == 2)
                        {
                            meritStudent.PaymentStatus = true;
                            meritStudent.PaymentTransactionId = payment.Id;
                            meritStudent.StudentCategoryId = payment.StudentCategoryId;
                            await _meritStudentManager.UpdateAsync(meritStudent);
                        }

                    }
                    AppliedStudent newStudent = await _appliedStudentManager.GetAppliedStudentAsync(payment.ReferenceNo, (int)payment.StudentCategoryId, (int)payment.AcademicSessionId);
                    phoneNumber = newStudent.MobileNo.ToString();
                    msgText = "Congratulations! " + newStudent.ApplicantName + "(NU Roll:" + newStudent.NUAdmissionRoll + ") , your admission payment is successfully paid";

                    //////////Code for SMS Sending and Saving///

                    bool SentSMS = false;
                    SentSMS = await ESMS.SendSMS(phoneNumber, msgText);
                    SMSModel newSMS = new SMSModel()
                    {
                        MobileList = phoneNumber,
                        Text = msgText,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "Payment Getway",
                        Description = "Payment Success"
                    };

                    if (SentSMS == true)
                    {
                        await _smsManager.AddAsync(newSMS);
                    }

                    return RedirectToAction("PaymentConfirmation", "Students", new
                    {
                        NuAdmissionRoll = payment.ReferenceNo,
                        CategoryId = payment.StudentCategoryId,
                        MeritTypeId = DBBL_Utilities.MeritTypeId,
                        academicSessionId = payment.AcademicSessionId,
                        paymentTransactionId = payment.Id,
                        notification = "Successful"
                    });
                }
                else
                {
                    return Ok();
                }
            }
            else
            {
                result.Add("status", "error");
                result.Add("message", "transaction result not found");
            }
            // var payment = await _paymentTransactionManager.GetPaymentTransactionAsync(model.NuRoll, model.CategoryId, model.SessionId, model.PaymentTypeId);

            //var data = transResult;
            return Ok();
        }
    }
}
