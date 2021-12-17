using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class PaymentType : BaseProps
    {
        [Required, Display(Name = "Payment Type Name"), StringLength(50)]
        public string PaymentTypeName { get; set; }
        [Required, Display(Name = "Short Code"), StringLength(4)]
        public string PaymentTypeShortCode { get; set; }
        public ICollection<StudentPaymentType> StudentPaymentTypes { get; set; }
    }
}
