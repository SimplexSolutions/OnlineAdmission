using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OnlineAdmission.APP.Utilities.NagadSetting;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;

namespace OnlineAdmission.APP.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTransactions : ControllerBase
    {
        private readonly IMeritStudentManager meritStudentManager;
        private readonly IPaymentTransactionManager paymentTransactionManager;
        private HttpClient httpClient;
        private readonly IAppliedStudentManager appliedStudentManager;
        private readonly ISubjectManager subjectManager;
        private readonly IStudentManager studentManager;

        public PaymentTransactions(IMeritStudentManager meritStudentManager, IPaymentTransactionManager paymentTransactionManager, IAppliedStudentManager appliedStudentManager,
            ISubjectManager subjectManager, IStudentManager studentManager)
        {
            this.meritStudentManager = meritStudentManager;
            this.paymentTransactionManager = paymentTransactionManager;
            this.appliedStudentManager = appliedStudentManager;
            this.subjectManager = subjectManager;
            this.studentManager = studentManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string status,string payment_ref_id)
        {
            // Status Check
            //Call Status Check API with Payment Ref ID
            var paymentDetails = "http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs//verify/payment/" + payment_ref_id;
            httpClient = new HttpClient(); 
            var responseContent =  await httpClient.GetAsync(paymentDetails);
            var  br_ResponseContent = await responseContent.Content.ReadAsStringAsync();
            dynamic responsevalue = JObject.Parse(br_ResponseContent);

            if (status.ToLower() == "success")
            {
              string  successNotification = "Congratulations! Payment Completed (BDT: " + responsevalue.amount+".00/-)";
                //Guid guid = new Guid();
                //{"merchantId":"683002007104225",
                //                "orderId":"3001639025135",
                //"paymentRefId":"MDgyNTE0NTEzNTQ1Ni42ODMwMDIwMDcxMDQyMjUuMzAwMTYzOTAyNTEzNS44MjcyM2EzZmU2N2NmM2M3MjNlNQ==",
                //"amount":"5",
                //"clientMobileNo":"016****7504",
                //"merchantMobileNo":"01300200710",
                //"orderDateTime":"2021-08-25 14:51:35.0",
                //"issuerPaymentDateTime":"2021-08-25 14:51:56.0",
                //"issuerPaymentRefNo":"0000Z1DW",
                //"additionalMerchantInfo":null,
                //"status":"Success",
                //"statusCode":"000",
                //"cancelIssuerDateTime":null,7
                //"cancelIssuerRefNo":null}
                var additionalMerchantInfo = (responsevalue.additionalMerchantInfo).Value;
                dynamic MerchantInfo = JObject.Parse(additionalMerchantInfo);

                //var existingMeritStudent = await meritStudentManager.GetByAdmissionRollAsync(Convert.ToInt32(GlobalVariables.nuRoll));
                //var existingAppliedStudent = await appliedStudentManager.GetByAdmissionRollAsync(Convert.ToInt32(GlobalVariables.nuRoll));
                //var existingSubject = await subjectManager.GetByCodeAsync(existingMeritStudent.SubjectCode);

                //string year = DateTime.Today.ToString("yyyy");
                //int subjectCode = existingSubject.Code;
                //int count = await studentManager.GetCountAsync(existingSubject.Id);
                //string sl = "";
                //if (count < 100)
                //{
                //    if (count == 0)
                //    {
                //        sl = "001";
                //    }
                //    else if (count < 10)
                //    {
                //        sl = "00" + count.ToString();
                //    }
                //    else if (count < 100 && count > 9)
                //    {
                //        sl = "0" + count.ToString();
                //    }
                //}
                //else
                //{
                //    sl = count.ToString();
                //}

                PaymentTransaction newPayment = new PaymentTransaction();
                
                newPayment.Amount = responsevalue.amount;
                newPayment.TransactionDate = DateTime.Today;
                newPayment.Balance = 0;
                newPayment.AccountNo = responsevalue.clientMobileNo;
                newPayment.TransactionId = responsevalue.orderId;
                newPayment.ReferenceNo =Convert.ToInt32(GlobalVariables.nuRoll);
                newPayment.AdmissionFee = MerchantInfo.AdmissionFee;
                newPayment.ServiceCharge = MerchantInfo.ServiceCharge;
                newPayment.ApplicantName = MerchantInfo.StudentName;
                newPayment.MobileNo = MerchantInfo.MobileNo;
                newPayment.SubjectId = MerchantInfo.SubjectId;

                await paymentTransactionManager.AddAsync(newPayment);

                MeritStudent meritStudent = await meritStudentManager.GetByAdmissionRollAsync(Convert.ToInt32(GlobalVariables.nuRoll));
                meritStudent.PaymentStatus = true;
                meritStudent.PaymentTransactionId = newPayment.Id;
                await meritStudentManager.UpdateAsync(meritStudent);


                // string site = "https://localhost:44356/";
                //string mySite = "http://115.127.26.3:4356/";
                //string param = "students/search?notification="+notification;
                //return Redirect(mySite + param);
                //return Redirect(site);
                // return Ok();
                //return RedirectToAction("search","students",new { notification=notification});
                return RedirectToAction("PaymentConfirmation", "Students",new { NuAdmissionRoll = newPayment.ReferenceNo,notification= successNotification});
            }

            return  Ok();
            //string site = "http://115.127.26.3:4356/";
            //string site = "https://localhost:44356/";
            //return Redirect(site);
        }

    }
}
