using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class AcademicSession : BaseProps
    {
        public string SessionName { get; set; }
        public ICollection<AppliedStudent> AppliedStudents { get; set; }
        public ICollection<StudentPaymentType> StudentPaymentTypes { get; set; }
    }
}
