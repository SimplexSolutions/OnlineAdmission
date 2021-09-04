using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineAdmission.APP.Utilities.NagadSetting
{
    class GlobalVariables
    {
        public static string MerchantId = "687231533088220";
        //public static string MerchantId = "683002007104225";
        //public static string nuRoll = "";
        //public static double AdmissionFee = 0.00;
        //public static double ServiceCharge = 0.00;


        private static DateTime DateTime = DateTime.Now;
        public static string RequestDateTime = DateTime.ToString("yyyyMMddHHmmss"); //{// Format should be 20200827134915

        //Generate Random Number
        static Random r = new Random();
        public static int RandomNumber = r.Next(100000000, 999999999); //Randam Number should be less than 20 char

        //Initialize API URL
        //public static string InitializeAPI = "http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/initialize/";
        public static string InitializeAPI = "https://api.mynagad.com/api/dfs/check-out/initialize/";



        //Example:  http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/initialize/" + this.merchantId + "/" + orderId;


        public static string CheckOutAPI = "https://api.mynagad.com/api/dfs/check-out/complete/";
        //public static string CheckOutAPI = "http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/complete/";


        //Example "http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/complete/" + sensitiveDataInitializeResponse.getPaymentReferenceId();

    }
}
