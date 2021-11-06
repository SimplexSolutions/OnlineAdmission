using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels.Student
{
    public class GetInfoVM
    {
        [Required, Display(Name = "NU Roll")]
        public int NUAdmissionRoll { get; set; }

        [Required,Display(Name = "Name")]
        public string ApplicantName { get; set; }

        [Required,Display(Name = "Father's Name")]
        public string FatherName { get; set; }

        [Required, Display(Name = "Mother's Name")]
        public string MotherName { get; set; }

        [Required, Display(Name = "Mobile")]
        public string MobileNo { get; set; }

        [Display(Name = "Group Name")]
        public string HSCGroup { get; set; }

    }
}
