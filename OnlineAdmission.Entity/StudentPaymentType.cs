using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class StudentPaymentType : BaseProps
    {
        public int StudentCategoryId { get; set; }
        public StudentCategory StudentCategory { get; set; }

        public int PaymentTypeId { get; set; }
        public PaymentType PaymentType { get; set; }

        public int AcademicSessionId { get; set; }
        public AcademicSession AcademicSession { get; set; }

        public bool Status { get; set; }
    }
}
