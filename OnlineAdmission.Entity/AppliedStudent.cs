using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class AppliedStudent 
    {
        public int Id { get; set; }

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
    }
}
