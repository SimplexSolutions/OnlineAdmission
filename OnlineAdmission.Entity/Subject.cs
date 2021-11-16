using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class Subject : BaseProps
    {
        
        [Display(Name ="Subject Name")]
        public string SubjectName { get; set; }

        [Display(Name = "Subject Code")]
        public int Code { get; set; }

        [Display(Name = "Admission Fee")]
        public double AdmissionFee { get; set; }

        public int? StudentCategoryId { get; set; }
        public StudentCategory StudentCategory { get; set; }
    }
}
