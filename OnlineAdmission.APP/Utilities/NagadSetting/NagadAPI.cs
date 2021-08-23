using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Utilities.NagadSetting
{
    public class NagadAPI
    {
        public HttpClient Initial()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(" http://sandbox.mynagad.com:10080");
            return client;
        }
    }
}
