using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class PaymentType : BaseProps
    {
        public string PaymentTypeName { get; set; }
        public string PaymentTypeShortCode { get; set; }
        public ICollection<StudentPaymentType> StudentPaymentTypes { get; set; }
    }
}
