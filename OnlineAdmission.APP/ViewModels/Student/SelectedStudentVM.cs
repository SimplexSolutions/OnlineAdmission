using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels.Student
{
    public class SelectedStudentVM
    {
        
        public MeritStudent MeritStudent { get; set; }
        public AppliedStudent AppliedStudent { get; set; }
        public Subject Subject { get; set; }

    }
}
