using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels.PaymentsVM
{
    public class SummeryReportVM
    {
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; }
        public ICollection<StudentCategory> StudentCategories { get; set; }
        public ICollection<PaymentType> PaymentTypes { get; set; }
    }
}
