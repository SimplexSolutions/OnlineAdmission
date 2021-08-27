using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public double Balance { get; set; }
        public string AccountNo { get; set; }
        public string TransactionId { get; set; }
        public int ReferenceNo { get; set; }
        public int CollegeRoll { get; set; }
    }
}
