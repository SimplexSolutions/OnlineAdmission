using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Web.Http;
//using System.Web.Configuration;
using Newtonsoft.Json;
//using System.Web.Mvc;
//using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using System.Configuration;
//using ServiceReference_Test;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
//using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
//using DBBLPaymentGateway.App.Service_References;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using OnlineAdmission.BLL.IManager;
using Microsoft.AspNetCore.Authorization;
using OnlineAdmission.APP.Utilities.DBBLUtilities;
using DBBL_ServiceLiveEcom1;
//using DBBL_ServiceTest;
using OnlineAdmission.Entity;
using OnlineAdmission.APP.Utilities.SMS;

namespace OnlineAdmission.APP.Controllers
{
    //[Route("[controller]/[action]")]
    //[ApiController]
    public class PaymentTransactionsDBBLController : apiController
    {
       
            private readonly IMeritStudentManager _meritStudentManager;
            private readonly IPaymentTransactionManager paymentTransactionManager;
            private readonly IAppliedStudentManager _appliedStudentManager;
            private readonly ISMSManager _smsManager;
            private IHttpContextAccessor _accessor;

            public PaymentTransactionsDBBLController(IMeritStudentManager meritStudentManager, IPaymentTransactionManager paymentTransactionManager, ISMSManager sMSManager,
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
            public async Task<IActionResult> GetTransactionResult(string trans_id,int cat_id)
            {
            if (cat_id == 1) 
            { 
                string site = "http://115.127.26.3:8020/PaymentTransactionsDBBL/GetTransactionResult?trans_id="+ trans_id;
                return new RedirectResult(site);
            }
                   

                

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

            //String RESULT_CODE = "N/A";
            //String DSECURE = "N/A";
            //String RRN = "N/A";
            //String APPROVAL_CODE = "N/A";
            //String CARD_NUMBER = "N/A";
            //String AMOUNT = "N/A";
            //String TRANS_DATE = "N/A";
            //String CARDNAME = "N/A";
            //String DESCRIPTION = "N/A";

            //string RESULT = transResult.ToString();

            //string[] resultfield = RESULT.Split('^');// .Split('^');
            //string part1 = "";
            //string part2 = "";

            //foreach (string lines in resultfield)
            //{
            //    part1 = lines.Split('>')[0].Trim();
            //    part2 = lines.Split('>')[1].Trim();

            //    if (part1.Contains("RESULT"))
            //    {
            //        RESULT = part2;
            //    }
            //    else if (part1.Contains("RESULT_CODE"))
            //    {
            //        RESULT_CODE = part2;
            //    }
            //    else if (part1.Contains("3DSECURE"))
            //    {
            //        DSECURE = part2;
            //    }

            //    else if (part1.Contains("RRN"))
            //    {
            //        RRN = part2;
            //    }
            //    else if (part1.Contains("APPROVAL_CODE"))
            //    {
            //        APPROVAL_CODE = part2;
            //    }
            //    else if (part1.Contains("CARD_NUMBER"))
            //    {
            //        CARD_NUMBER = part2;
            //    }
            //    else if (part1.Contains("AMOUNT"))
            //    {
            //        AMOUNT = part2;
            //    }
            //    else if (part1.Contains("TRANS_DATE"))
            //    {
            //        TRANS_DATE = part2;
            //    }
            //    else if (part1.Contains("CARDNAME"))
            //    {
            //        CARDNAME = part2;
            //    }
            //    else if (part1.Contains("DESCRIPTION"))
            //    {
            //        DESCRIPTION = part2;
            //    }

            //}

            //Console.WriteLine("RESULT:" + RESULT);
            //Console.WriteLine("RESULT_CODE:" + RESULT_CODE);
            //Console.WriteLine("DSECURE:" + DSECURE);
            //Console.WriteLine("RRN:" + RRN);
            //Console.WriteLine("APPROVAL_CODE:" + APPROVAL_CODE);
            //Console.WriteLine("CARD_NUMBER:" + CARD_NUMBER);
            //Console.WriteLine("AMOUNT:" + AMOUNT);
            //Console.WriteLine("TRANS_DATE:" + TRANS_DATE);
            //Console.WriteLine("CARDNAME:" + CARDNAME);
            //Console.WriteLine("DESCRIPTION:" + DESCRIPTION);

            if (transResult.Body.@return.Contains("OK"))
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
                        return new RedirectToActionResult("PaymentConfirmation", "Students", new
                        {
                            NuAdmissionRoll = payment.ReferenceNo,
                            CategoryId = (int)payment.StudentCategoryId,
                            MeritTypeId = DBBL_Utilities.MeritTypeId,
                            academicSessionId = payment.AcademicSessionId,
                            paymentTransactionId = payment.Id,
                            notification = msgText
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

        private IActionResult Redirect(string v)
        {
            throw new NotImplementedException();
        }

        private IActionResult RedirectToAction(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        private IActionResult Ok()
        {
            throw new NotImplementedException();
        }

        private IActionResult RedirectToAction(string v1, string v2, object p)
        {
            throw new NotImplementedException();
        }
    }
    }
