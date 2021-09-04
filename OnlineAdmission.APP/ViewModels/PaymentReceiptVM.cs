using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels
{
    public class PaymentReceiptVM
    {
        public PaymentTransaction PaymentTransaction { get; set; }
        public Subject Subject { get; set; }
        public MeritStudent MeritStudent { get; set; }
        public AppliedStudent AppliedStudent { get; set; }
        public OnlineAdmission.Entity.Student Student { get; set; }
    }
}
