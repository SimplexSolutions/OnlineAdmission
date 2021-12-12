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
        public ICollection<StudentPaymentType> StudentPaymentTypes { get; set; }
        public ICollection<AppliedStudent> AppliedStudents { get; set; }
        public ICollection<MeritStudent> MeritStudents { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; }
    }
}
