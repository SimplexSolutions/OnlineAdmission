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
        private readonly IMeritStudentManager _meritStudentManager;
        private readonly IPaymentTransactionManager paymentTransactionManager;
        private HttpClient httpClient;
        private readonly IAppliedStudentManager _appliedStudentManager;
        private readonly IStudentManager _studentManager;
        private readonly ISMSManager _smsManager;

        public PaymentTransactions(IMeritStudentManager meritStudentManager, IPaymentTransactionManager paymentTransactionManager, ISMSManager sMSManager, 
            IStudentManager studentManager, IAppliedStudentManager appliedStudentManager)
        {
            _meritStudentManager = meritStudentManager;
            this.paymentTransactionManager = paymentTransactionManager;
            _appliedStudentManager = appliedStudentManager;
            _studentManager = studentManager;
            _smsManager = sMSManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string status,string payment_ref_id)
        {

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

                int nuRoll = MerchantInfo.NuAdmissionRoll;
                int stuCat = MerchantInfo.StudentCategory;
                int patType = MerchantInfo.PaymentType;
                int sessionId = MerchantInfo.sessionId;
                int meritTypeId = MerchantInfo.meritType;

                PaymentTransaction newPayment = new PaymentTransaction {
                    Amount = responsevalue.amount,
                    TransactionDate = DateTime.Today,
                    Balance = 0,
                    AccountNo = responsevalue.clientMobileNo,
                    TransactionId = responsevalue.orderId,
                    ReferenceNo = nuRoll,
                    AdmissionFee = MerchantInfo.AdmissionFee,
                    ServiceCharge = MerchantInfo.ServiceCharge,
                    StudentName = MerchantInfo.StudentName,
                    MobileNumber = MerchantInfo.MobileNo,
                    StudentCategoryId = stuCat,
                    PaymentTypeId = patType,
                    AcademicSessionId = sessionId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = HttpContext.Session.GetString("UserId")
                };
                
                //newPayment.ApplicantName = MerchantInfo.StudentName;
                //newPayment.MobileNo = MerchantInfo.MobileNo;
                //newPayment.SubjectId = MerchantInfo.SubjectId;
                if (responsevalue.subjectChange == 1)
                {
                   // MeritStudent existingMeritStudent = await meritStudentManager.GetHonsByAdmissionRollAsync(MerchantInfo.NuAdmissionRoll);
                    MeritStudent existingMeritStudent= await _meritStudentManager.GetMeritStudentAsync(MerchantInfo.MerchantInfo.NuAdmissionRoll, MerchantInfo.StudentCategory,
                        MerchantInfo.meritType, MerchantInfo.sessionId);
                    //var existingStudent = await _studentManager.GetStudentByHSCRollAsync(existingMeritStudent.HSCRoll);
                    var existingStudent = await _studentManager.GetStudentAsync(MerchantInfo.NuAdmissionRoll, MerchantInfo.StudentCategory, MerchantInfo.sessionId);
                    existingStudent.Status = true;
                    await _studentManager.UpdateAsync(existingStudent);
                }

                PaymentTransaction exPT = await paymentTransactionManager.GetPaymentTransactionByTrId(newPayment.TransactionId);
                if (exPT!=null)
                {
                    return RedirectToAction("Search", "Students");
                    //if (newPayment.StudentCategoryId==1)
                    //{

                    //    return RedirectToAction("Search", "Students");
                    //}
                    //else if (newPayment.StudentCategoryId==2)
                    //{
                    //    return RedirectToAction("ProfessionalSearch", "Students");
                    //}
                    //else if (newPayment.StudentCategoryId == 3)
                    //{
                    //    return RedirectToAction("MastersSearch", "Students");
                    //}
                    //else if (newPayment.StudentCategoryId == 4)
                    //{
                    //    return RedirectToAction("MastersSearchGeneral", "Students");
                    //}
                    //else if (newPayment.StudentCategoryId == 5)
                    //{
                    //    return RedirectToAction("DegreeSearch", "Students");
                    //}
                }
                await paymentTransactionManager.AddAsync(newPayment);
                //MeritStudent meritStudent = new MeritStudent();
                //meritStudent = await _meritStudentManager.GetMeritStudentAsync((int)MerchantInfo.NuAdmissionRoll, (int)MerchantInfo.StudentCategory,
                //        MerchantInfo.meritType, MerchantInfo.sessionId);
                //var meritStudent = await _meritStudentManager.GetMeritStudentAsync(MerchantInfo.NuAdmissionRoll, MerchantInfo.StudentCategory,
                //    MerchantInfo.meritType, MerchantInfo.sessionId);
                //var existingStudent2 = await _studentManager.GetStudentAsync(MerchantInfo.NuAdmissionRoll, MerchantInfo.StudentCategory, MerchantInfo.sessionId);

                MeritStudent meritStudent = new MeritStudent();
                 meritStudent = await _meritStudentManager.GetMeritStudentAsync(nuRoll, stuCat, meritTypeId, sessionId);
                // var meritStudent = await meritStudentManager.GetMeritStudentAsync(model.NuRoll, model.CategoryId, model.MeritTypeId, model.SessionId);

                //if (newPayment.StudentCategory == 1)
                //{
                //    meritStudent = await meritStudentManager.GetByAdmissionRollAsync(Convert.ToInt32(MerchantInfo.NuAdmissionRoll), (int)newPayment.StudentCategory, MerchantInfo.meritType);
                //}
                //else if (newPayment.StudentCategory == 2)
                //{
                //    meritStudent = await meritStudentManager.GetProByAdmissionRollAsync(Convert.ToInt32(MerchantInfo.NuAdmissionRoll));
                //}
                //else if (newPayment.StudentCategory == 3)
                //{
                //    meritStudent = await meritStudentManager.GetProMBAByAdmissionRollAsync(Convert.ToInt32(MerchantInfo.NuAdmissionRoll));
                //}
                //else if (newPayment.StudentCategory == 4)
                //{
                //    meritStudent = await meritStudentManager.GetGenMastersByAdmissionRollAsync(Convert.ToInt32(MerchantInfo.NuAdmissionRoll));
                //}
                //else if (newPayment.StudentCategory == 5)
                //{
                //    meritStudent = await meritStudentManager.GetDegreeByAdmissionRollAsync(Convert.ToInt32(MerchantInfo.NuAdmissionRoll));
                //}

                //MeritStudent meritStudent = await nagadManager.GetMeritStudentByNURollNagad(Convert.ToInt32(MerchantInfo.NuAdmissionRoll));
                string phoneNumber ;
                string msgText;

                phoneNumber = MerchantInfo.MobileNo;
                msgText = "Congratulations! your payment has been successfully paid";

                //For Common Code
                if (meritStudent != null)
                {
                    if (newPayment.PaymentTypeId==2)
                    {
                        meritStudent.PaymentStatus = true;
                        meritStudent.PaymentTransactionId = newPayment.Id;
                        meritStudent.StudentCategoryId = newPayment.StudentCategoryId;
                        await _meritStudentManager.UpdateAsync(meritStudent);
                    }                   
                    
                }


                //if (newPayment.StudentCategoryId == 1) //For Hon's General Student
                //{
                //    //AppliedStudent newStudent = await appliedStudentManager.GetByAdmissionRollAsync(meritStudent.NUAdmissionRoll);
                //    //AppliedStudent newStudent = await _appliedStudentManager.GetByAdmissionRollAsync(Convert.ToInt32(MerchantInfo.NuAdmissionRoll), (int)newPayment.StudentCategoryId);
                //    AppliedStudent newStudent = await _appliedStudentManager.GetAppliedStudentAsync((int)MerchantInfo.NuAdmissionRoll, (int)newPayment.StudentCategoryId, MerchantInfo.sessionId);
                //    phoneNumber = newStudent.MobileNo.ToString();
                //    msgText = "Congratulations! " + newStudent.ApplicantName + "(NU Roll:" + newStudent.NUAdmissionRoll + ") , your admission payment is successfully paid";
                //}
               // nuRoll, stuCat, meritTypeId, sessionId

                AppliedStudent newStudent = await _appliedStudentManager.GetAppliedStudentAsync(nuRoll,stuCat, sessionId);
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

                return RedirectToAction("PaymentConfirmation", "Students", new { NuAdmissionRoll = newPayment.ReferenceNo,
                    CategoryId= stuCat,MeritTypeId=meritTypeId, academicSessionId =sessionId,
                    paymentTransactionId = newPayment.Id, notification = successNotification });

                //if (MerchantInfo.StudentType == 1)
                //{
                //    return RedirectToAction("PaymentConfirmation", "Students", new { NuAdmissionRoll = newPayment.ReferenceNo, notification = successNotification });
                //}
                //if (MerchantInfo.StudentType==2)
                //{
                //    return RedirectToAction("ProfessionalSearch", "Students", new { professionalRoll = newPayment.ReferenceNo, notification = successNotification });
                //}
                //if (MerchantInfo.StudentType==3)
                //{
                //    return RedirectToAction("MastersSearch", "Students", new { mastersRoll = newPayment.ReferenceNo, notification = successNotification });
                //}
                //if (MerchantInfo.StudentType == 4)
                //{
                //    return RedirectToAction("MastersSearchGeneral", "Students", new { mastersRoll = newPayment.ReferenceNo, notification = successNotification });
                //}
                //if (MerchantInfo.StudentType == 5)
                //{
                //    return RedirectToAction("DegreeSearch", "Students", new { degreePassRoll = newPayment.ReferenceNo, notification = successNotification });
                //}
            }

            return  Ok();
        }

        
    }
}
