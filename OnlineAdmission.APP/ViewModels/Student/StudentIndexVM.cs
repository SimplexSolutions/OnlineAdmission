using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels.Student
{
    public class StudentIndexVM
    {
        public List<Entity.Student> Students { get; set; }
        public StudentCategory StudentCategory { get; set; }
    }
}
