using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.Entity;

namespace OnlineAdmission.APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTransactions : ControllerBase
    {
        private readonly IMeritStudentManager meritStudentManager;
        private readonly IPaymentTransactionManager paymentTransactionManager;
        private HttpClient httpClient;

        public PaymentTransactions(IMeritStudentManager meritStudentManager, IPaymentTransactionManager paymentTransactionManager)
        {
            this.meritStudentManager = meritStudentManager;
            this.paymentTransactionManager = paymentTransactionManager;
        }
        [HttpGet]
        public async Task<IActionResult> Get(string status,string notification,string amount, string payment_ref_id,string nuRoll)
        {
            // Status Check
            //Call Status Check API with Payment Ref ID
            var paymentDetails = "http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs//verify/payment/" + payment_ref_id;
            httpClient = new HttpClient(); 
          var adsfa=  await httpClient.GetAsync(paymentDetails);

           var  br_ResponseContent = await adsfa.Content.ReadAsStringAsync();


            if (status.ToLower() == "success")
            {
                //Guid guid = new Guid();
                notification = "Payment successfull";
                string admissionRoll= nuRoll;
                //string abcd =UTF8Encoding payment_ref_id ;
                //var br_httpContent = new StringContent(payment_ref_id, Encoding.UTF8, "application/json");
                //var ddd = br_httpContent .ToString();
                //var returnDcriptedData= StudentsController.Decrypt(ddd);
                //var returnDcriptedData = StudentsController.Decrypt(payment_ref_id);

                // var response = payment_ref_id.Body.ToString();

                //dynamic response = JObject.Parse(returnDcriptedData);
                // string returnedSensitiveData = response.sensitiveData;

                //string returnedSignature = response.signature;

                //Decrypt Sensitive Data
                //string decryptedSensitiveData =StudentsController.Decrypt(adsfas);



                //Process Decrypted Data
                //  dynamic responsevalue = JObject.Parse(decryptedSensitiveData);

                // string challenge = responsevalue.challenge;
                // string paymentRefId = responsevalue.paymentReferenceId;




                //dynamic co_Response = JObject.Parse(payment_ref_id);

                //PaymentTransaction newPayment = new PaymentTransaction();

                //newPayment.Amount = Convert.ToInt32(amount);
                //newPayment.TransactionDate = DateTime.Today;
                //newPayment.Balance = 0;
                ////newPayment.AccountNo = model.PaymentTransaction.AccountNo;
                ////var guid = Guid.NewGuid();
                //newPayment.TransactionId = guid.ToString();
                ////newPayment.ReferenceNo = Convert.ToInt32(nuRoll);

                //await paymentTransactionManager.AddAsync(newPayment);

                //MeritStudent meritStudent = await meritStudentManager.GetByAdmissionRollAsync(Convert.ToInt32(nuRoll));
                //meritStudent.PaymentStatus = true;
                //meritStudent.PaymentTransactionId = newPayment.Id;
                //await meritStudentManager.UpdateAsync(meritStudent);
                // string site = "https://localhost:44356/";
                //string mySite = "http://115.127.26.3:4356/";
                //string param = "students/search?notification="+notification;
                //return Redirect(mySite + param);
                //return Redirect(site);
               // return Ok();
                return RedirectToAction("search","students",new { notification=notification+"_"+ payment_ref_id });
            }

            return  Ok();
            //string site = "http://115.127.26.3:4356/";
            //string site = "https://localhost:44356/";
            //return Redirect(site);
        }


    }
}
