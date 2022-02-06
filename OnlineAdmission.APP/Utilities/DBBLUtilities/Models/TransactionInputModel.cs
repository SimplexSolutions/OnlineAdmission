using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineAdmission.APP.Utilities.DBBLUtilities.Models
{
    public class TransactionInputModel
    {
        public string Amount { get; set; }
        public string Cardtype { get; set; }
        public string Txnrefnum { get; set; }
        public string Clientip { get; set; }
    }
}