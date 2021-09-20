using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels.Student
{
    public class SubjectChangedVM
    {
        
        public int StudentId { get; set; }
        public int SubjectId { get; set; }

        public ICollection<SelectListItem> StudentList { get; set; }
        public ICollection<SelectListItem> SubjectList { get; set; }
    }
}
