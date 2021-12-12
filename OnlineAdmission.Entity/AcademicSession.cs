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
        public ICollection<MeritStudent> MeritStudents { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<StudentPaymentType> StudentPaymentTypes { get; set; }
    }
}
