using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public double Amount { get; set; }

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

        //StudentType = null = hon's Student
        //StudentType = 1 = professional Student
        public int? StudentType { get; set; }  
    }
}
