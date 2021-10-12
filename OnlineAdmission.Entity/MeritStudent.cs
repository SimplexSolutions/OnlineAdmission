using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class MeritStudent
    {
        public int Id { get; set; }

        [Display(Name = "NU Roll")]
        public int NUAdmissionRoll { get; set; }


        [Display(Name = "HSC Roll")]
        public int HSCRoll { get; set; }

        [Display(Name = "Subject Code")]
        public int SubjectCode { get; set; } //subject Code=0 will consider as applied student otherwise selected student


        [Display(Name = "Merit Position")]
        public int MeritPosition { get; set; }

        public string Comments { get; set; }

        [Display(Name = "Is Paid")]
        public bool PaymentStatus { get; set; } = false;

        //[Display(Name = "Amount")]
        //public double PaidAmaount { get; set; }

        [Display(Name = "Payment Transaction Id")]
        public int? PaymentTransactionId { get; set; }

        [Display(Name = "Deduction")]
        public double DeductedAmaount { get; set; }        

        public PaymentTransaction PaymentTransaction { get; set; }

        //StudentCategory = 1 = hon's Student
        //StudentCategory = 2 = professional Student
        //StudentCategory = 3 = Masters
        public int? StudentCategory { get; set; }
    }
}
