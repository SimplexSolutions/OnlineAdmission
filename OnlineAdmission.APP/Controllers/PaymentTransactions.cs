﻿using System;
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
using OnlineAdmission.APP.Utilities.SMS;
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
       
        //private readonly IAppliedStudentManager appliedStudentManager;
        //private readonly ISubjectManager subjectManager;
        //private readonly IStudentManager studentManager;
        private readonly INagadManager nagadManager;
        


        private readonly ISMSManager _smsManager;

        public PaymentTransactions(IMeritStudentManager meritStudentManager,IPaymentTransactionManager paymentTransactionManager, ISMSManager sMSManager,INagadManager nagadManager)
        {
            this.meritStudentManager = meritStudentManager;
            this.paymentTransactionManager = paymentTransactionManager;
            //this.appliedStudentManager = appliedStudentManager;
            //this.subjectManager = subjectManager;
            //this.studentManager = studentManager;
            this.nagadManager = nagadManager;
            _smsManager = sMSManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string status,string payment_ref_id)
        {
            //AppliedStudent appliedStudent = await nagadManager.GetAppliedStudentByNURollNagad(nuRoll);
            //MeritStudent meritStudent = await nagadManager.GetMeritStudentByNURollNagad(nuRoll);
            //Subject subject = await nagadManager.GetSubjectByCodeNagad(meritStudent.SubjectCode);
            // Status Check
            //Call Status Check API with Payment Ref ID
            var paymentDetails = "https://api.mynagad.com/api/dfs/verify/payment/" + payment_ref_id;
            //var paymentDetails = "http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs//verify/payment/" + payment_ref_id;
            httpClient = new HttpClient(); 
            var responseContent =  await httpClient.GetAsync(paymentDetails);
            var  br_ResponseContent = await responseContent.Content.ReadAsStringAsync();
            dynamic responsevalue = JObject.Parse(br_ResponseContent);

            if (status.ToLower() == "success")
            {
                string successNotification = "Congratulations! Payment Completed (BDT: " + responsevalue.amount + "/-)";

                var additionalMerchantInfo = (responsevalue.additionalMerchantInfo).Value;
                dynamic MerchantInfo = JObject.Parse(additionalMerchantInfo);

                
                PaymentTransaction newPayment = new PaymentTransaction{
                    Amount = responsevalue.amount,
                    TransactionDate = DateTime.Today,
                    Balance = 0,
                    AccountNo = responsevalue.clientMobileNo,
                    TransactionId = responsevalue.orderId,
                    ReferenceNo = MerchantInfo.NuAdmissionRoll,
                    AdmissionFee = MerchantInfo.AdmissionFee,
                    ServiceCharge = MerchantInfo.ServiceCharge,
                    StudentName = MerchantInfo.studentName,
                    MobileNumber = MerchantInfo.MobileNo,
                    StudentCategory = MerchantInfo.StudentCategory,
                    PaymentType = MerchantInfo.PaymentType
                    };
                //newPayment.ApplicantName = MerchantInfo.StudentName;
                //newPayment.MobileNo = MerchantInfo.MobileNo;
                //newPayment.SubjectId = MerchantInfo.SubjectId;

                await paymentTransactionManager.AddAsync(newPayment);
                
                //MeritStudent meritStudent = await meritStudentManager.GetByAdmissionRollAsync(Convert.ToInt32(MerchantInfo.NuAdmissionRoll));
                MeritStudent meritStudent = await nagadManager.GetMeritStudentByNURollNagad(Convert.ToInt32(MerchantInfo.NuAdmissionRoll));
                string phoneNumber ;
                string msgText;


                if (newPayment.StudentCategory == 1)
                {
                    phoneNumber = MerchantInfo.MobileNo;
                    msgText = "Congratulations! your payment is successfully paid";
                }
                else if (newPayment.StudentCategory == 2)
                {
                    phoneNumber = MerchantInfo.MobileNo;
                    msgText = "Congratulations! your payment is successfully paid";
                    //var AdmissionPayment = await _paymentTransactionManager.GetAdmissionTrByNuRoll(professionalRoll);
                    if (meritStudent != null) 
                    {
                        meritStudent.PaymentStatus = true;
                        meritStudent.PaymentTransactionId = newPayment.Id;
                        await meritStudentManager.UpdateAsync(meritStudent);
                    }
                }
                else
                {
                    meritStudent.PaymentStatus = true;
                    meritStudent.PaymentTransactionId = newPayment.Id;
                    await meritStudentManager.UpdateAsync(meritStudent);  
                    //AppliedStudent newStudent = await appliedStudentManager.GetByAdmissionRollAsync(meritStudent.NUAdmissionRoll);
                    AppliedStudent newStudent = await nagadManager.GetAppliedStudentByNURollNagad(Convert.ToInt32(MerchantInfo.NuAdmissionRoll));
                    phoneNumber = newStudent.MobileNo.ToString();
                    msgText = "Congratulations! " + newStudent.ApplicantName + "(NU Roll:" + newStudent.NUAdmissionRoll + ") , your admission payment is successfully paid";
                }
                

                //////////////////Code for SMS Sending and Saving
                ///
                
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


                if (MerchantInfo.StudentType==1)
                {
                    return RedirectToAction("ProfessionalSearch", "Students", new { professionalRoll = newPayment.ReferenceNo, notification = successNotification });
                }
                if (MerchantInfo.StudentType==2)
                {
                    return RedirectToAction("MastersSearch", "Students", new { mastersRoll = newPayment.ReferenceNo, notification = successNotification });
                }
                return RedirectToAction("PaymentConfirmation", "Students",new { NuAdmissionRoll = newPayment.ReferenceNo,notification= successNotification});
            }

            return  Ok();
        }

        
    }
}
