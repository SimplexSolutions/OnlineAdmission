using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DBBL_ServiceLiveEcom1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OnlineAdmission.APP.Utilities.DBBLUtilities;
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
        private IHttpContextAccessor _accessor;

        public PaymentTransactions(IMeritStudentManager meritStudentManager, IPaymentTransactionManager paymentTransactionManager, ISMSManager sMSManager, 
            IStudentManager studentManager, IAppliedStudentManager appliedStudentManager, IHttpContextAccessor accessor)
        {
            _meritStudentManager = meritStudentManager;
            this.paymentTransactionManager = paymentTransactionManager;
            _appliedStudentManager = appliedStudentManager;
            _studentManager = studentManager;
            _smsManager = sMSManager;
            _accessor = accessor;
        }

        //#region DBBL
        ////ActionResult for DBBL Payment
        ////[Route("api/[controller]/[action]")]
        ////[ApiController]
        ////[Route("api/[controller]/[action]")]
        //[HttpGet]        
        //public async Task<IActionResult> GetTransactionResult(string trans_id)
        //{
        //    var result = new Dictionary<string, object>();

        //    string _user = string.Empty;
        //    string _pass = string.Empty;


        //    _user = DBBL_Utilities.user; 
        //    _pass = DBBL_Utilities.pass;


        //    string Clintip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
        //    string Txnrefnum = "Test";

        //    ServicePointManager.Expect100Continue = true;

        //    var dbbl = new dbblecomtxnClient();
        //    var transResult = await dbbl.getresultfieldAsync(_user, _pass, trans_id, Clintip, Txnrefnum);
        //    if (transResult.Body.@return.Length>0)
        //    {
        //        var payment = await paymentTransactionManager.GetPaymentTransactionByTrId(trans_id);
        //        if (payment != null)
        //        {
        //            payment.PaymentStatus = true;
        //            await paymentTransactionManager.UpdateAsync(payment);

        //            MeritStudent meritStudent = new MeritStudent();
        //            meritStudent = await _meritStudentManager.GetMeritStudentAsync(payment.ReferenceNo, (int)payment.StudentCategoryId, DBBL_Utilities.MeritTypeId, (int)payment.AcademicSessionId);
        //            string phoneNumber;
        //            string msgText;

        //            phoneNumber = payment.MobileNumber;
        //            msgText = "Congratulations! your payment has been successfully paid";

        //            if (meritStudent != null)
        //            {
        //                if (payment.PaymentTypeId == 2)
        //                {
        //                    meritStudent.PaymentStatus = true;
        //                    meritStudent.PaymentTransactionId = payment.Id;
        //                    meritStudent.StudentCategoryId = payment.StudentCategoryId;
        //                    await _meritStudentManager.UpdateAsync(meritStudent);
        //                }

        //            }
        //            AppliedStudent newStudent = await _appliedStudentManager.GetAppliedStudentAsync(payment.ReferenceNo, (int)payment.StudentCategoryId, (int)payment.AcademicSessionId);
        //            phoneNumber = newStudent.MobileNo.ToString();
        //            msgText = "Congratulations! " + newStudent.ApplicantName + "(NU Roll:" + newStudent.NUAdmissionRoll + ") , your admission payment is successfully paid";

        //            //////////Code for SMS Sending and Saving///

        //            bool SentSMS = false;
        //            SentSMS = await ESMS.SendSMS(phoneNumber, msgText);
        //            SMSModel newSMS = new SMSModel()
        //            {
        //                MobileList = phoneNumber,
        //                Text = msgText,
        //                CreatedAt = DateTime.Now,
        //                CreatedBy = "Payment Getway",
        //                Description = "Payment Success"
        //            };

        //            if (SentSMS == true)
        //            {
        //                await _smsManager.AddAsync(newSMS);
        //            }

        //            return RedirectToAction("PaymentConfirmation", "Students", new
        //            {
        //                NuAdmissionRoll = payment.ReferenceNo,
        //                CategoryId = payment.StudentCategoryId,
        //                MeritTypeId = DBBL_Utilities.MeritTypeId,
        //                academicSessionId = payment.AcademicSessionId,
        //                paymentTransactionId = payment.Id,
        //                notification = "Successful"
        //            });
        //        }
        //        else 
        //        {
        //            return Ok();
        //        }
        //    }
        //    else
        //    {
        //        result.Add("status", "error");
        //        result.Add("message", "transaction result not found");
        //    }
        //    // var payment = await _paymentTransactionManager.GetPaymentTransactionAsync(model.NuRoll, model.CategoryId, model.SessionId, model.PaymentTypeId);

        //    //var data = transResult;
        //    return Ok();
        //}

        //#endregion

        #region Nagad
        //ActionResult for Nagad payment
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
                //if (MerchantInfo.studentType == 1)
                //{
                //    string site = "http://115.127.26.3:8020/api/PaymentTransactionsAPI?status=" + status+ "&payment_ref_id=" + payment_ref_id;
                //    return new RedirectResult(site);
                //}

                int nuRoll = MerchantInfo.NuAdmissionRoll;
                int stuCat = MerchantInfo.StudentCategory;
                int patType = MerchantInfo.PaymentType;
                int sessionId = MerchantInfo.sessionId;
                int meritTypeId = MerchantInfo.meritType;

                PaymentTransaction newPayment = new PaymentTransaction {
                    Amount = responsevalue.amount,
                    TransactionDate = DateTime.Now,
                    Balance = 0,
                    AccountNo = responsevalue.clientMobileNo,
                    TransactionId = responsevalue.issuerPaymentRefNo,
                    Description = responsevalue.orderId,
                    ReferenceNo = nuRoll,
                    AdmissionFee = MerchantInfo.AdmissionFee,
                    ServiceCharge = MerchantInfo.ServiceCharge,
                    PaymentStatus=true,
                    StudentName = MerchantInfo.StudentName,
                    MobileNumber = MerchantInfo.MobileNo,
                    StudentCategoryId = stuCat,
                    PaymentTypeId = patType,
                    AcademicSessionId = sessionId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "Nagad"
                //CreatedBy = HttpContext.Session.GetString("UserId")
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

        #endregion

    }
}
