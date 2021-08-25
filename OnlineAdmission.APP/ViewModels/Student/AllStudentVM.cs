using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels.Student
{
    public class AllStudentVM
    {
        public List<AppliedStudent> AppliedStudents { get; set; }
        public List<MeritStudent> MeritStudents { get; set; }
        public List<OnlineAdmission.Entity.Student> Students { get; set; }

        //public List<MeritStudentWithSubjectVM> MeritStudentWithSubjectVMs { get; set; }
    }
}
