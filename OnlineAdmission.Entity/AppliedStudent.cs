using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class AppliedStudent : BaseProps
    {

        [Display(Name = "NU Roll")]

        [Required]
        public int NUAdmissionRoll { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string ApplicantName { get; set; }

        [Display(Name = "Father's Name")]
        [Required]
        public string FatherName { get; set; }

        [Display(Name = "Mother's Name")]
        [Required]
        public string MotherName { get; set; }

        [Display(Name = "Mobile")]
        [Required]
        public string MobileNo { get; set; }

        [Display(Name = "Group Name")]
        public string HSCGroup { get; set; }

        public int? AcademicSessionId { get; set; }
        public AcademicSession AcademicSession { get; set; }

        public int? StudentCategoryId { get; set; }
        public StudentCategory StudentCategory { get; set; }
    }
}
