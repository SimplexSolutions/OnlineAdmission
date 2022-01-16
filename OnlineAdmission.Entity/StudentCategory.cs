using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class StudentCategory: BaseProps
    {
        [Display(Name="Category")]
        public string CategoryName { get; set; }
        [Required, Display(Name = "Short Code"), StringLength(4)]
        public string CategoryShortCode { get; set; }
        public double ApplicationFee { get; set; }

        [StringLength(7)]
        public string IDCardShortName { get; set; }
        public ICollection<StudentPaymentType> StudentPaymentTypes { get; set; }
        public ICollection<AppliedStudent> AppliedStudents { get; set; }
        public ICollection<MeritStudent> MeritStudents { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; }
    }
}
