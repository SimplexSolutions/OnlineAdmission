using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels.Student
{
    public class StudentDetailsVM
    {
        public OnlineAdmission.Entity.Student Student { get; set; }
        public MeritStudent MeritStudent { get; set; }
        public Subject PreviousSubject { get; set; }
    }
}
