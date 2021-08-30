using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Utilities.SMS
{
    public class ESMS
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<bool> SendSMS(string no, string msg)
        {
            try
            {
                var values = new Dictionary<string, string>
                {
                   //{ "token", "b6f7a999a5c3fab2bb343262c7a1e0a1" },
                   { "to", "01717678134" },
                   { "message", "Hello we are testing our sms solution from inhouse testing team. Allah Hafiz." }
                };

                //var values = new Dictionary<string, string>
                //{
                //   { "token", "b6f7a999a5c3fab2bb343262c7a1e0a1" },
                //   { "to", no },
                //   { "message", msg }
                //};

                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://api.greenweb.com.bd/api.php?", content);
                var responseString = await response.Content.ReadAsStringAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
