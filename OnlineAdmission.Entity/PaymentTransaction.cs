using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class PaymentTransaction : BaseProps
    {
        public double Amount { get; set; }

        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [Display(Name = "Mobile")]
        public string MobileNumber { get; set; }

        [Display(Name ="Paid Date")]
        public DateTime TransactionDate { get; set; }
        public double Balance { get; set; }

        [Display(Name = "Account No")]
        public string AccountNo { get; set; }

        [Display(Name = "Transaction Id")]
        public string TransactionId { get; set; }
        
        [Display(Name = "Reference")]
        public int ReferenceNo { get; set; }

        [Display(Name = "Admission Fee")]
        public double AdmissionFee { get; set; }

        [Display(Name = "Service Charge")]
        public double ServiceCharge { get; set; }

        public string Description { get; set; }
        [Display(Name = "Is Paid ?")]
        public bool PaymentStatus { get; set; } = false;

        //StudentCategory = 1 = hon's Student
        //StudentCategory = 2 = professional Student
        //StudentCategory = 3 = Masters
        public int? StudentCategoryId { get; set; }
        public StudentCategory StudentCategory { get; set; }

        //Application Fee = 1
        //Addmission Fee = 2
        [Display(Name = "Payment Type")] 
        public int? PaymentTypeId { get; set; }
        //public PaymentType PaymentType { get; set; }

        [Display(Name ="Academic Session")]
        public int? AcademicSessionId { get; set; }
        public AcademicSession AcademicSession { get; set; }
    }
}
