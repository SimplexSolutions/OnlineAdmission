using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels
{
    public class TransactionInfo
    {
        public string merchantId { get; set; }
        public string orderId { get; set; }
        public int NuRoll { get; set; }
        public double Amount { get; set; }
        public string Name { get; set; }
        public string SubjectName { get; set; }
        public Subject Subject { get; set; }
        public PaymentTransaction PaymentTransaction { get; set; }
    }
}
