using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels.Student
{
    public class StudentCreateVM
    {

        [Display(Name = "Subject")]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        //[Display(Name = "District")]
        //public int DistrictId { get; set; }
        public int StudentCategory { get; set; }
        [Required, StringLength(150)]
        public string Name { get; set; }

        //[Required]
        public string Photo { get; set; }

        [Required, Display(Name = "Father's Name"), StringLength(250)]
        public string FatherName { get; set; }

        [Display(Name = "Occupation")]
        public string FatherOccupation { get; set; }

        [Required, Display(Name = "Mother's Name"), StringLength(250)]
        public string MotherName { get; set; }

        [Display(Name = "Occupation")]
        public string MotherOccupation { get; set; }

        [Display(Name = "Guardian's Name"), StringLength(250)]
        public string GuardianName { get; set; }

        [Display(Name = "Occupation")]
        public string GuardianOccupation { get; set; }

        [Required]
        public string Nationality { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(10)]
        public string BloodGroup { get; set; }

        [Display(Name = "Present Address"), Required]
        public string PresentAddress1 { get; set; }
        public string PresentAddress2 { get; set; }

        [Display(Name = "District")]
        public int PresentDistrictId { get; set; }

        [Display(Name = "Permanent Address"), Required]
        public string PermanentAddress1 { get; set; }
        public string PermanentAddress2 { get; set; }

        [Display(Name = "District")]
        public int PermanentDistrictId { get; set; }

        [Display(Name = "Village"), Required]
        public string MailingVillage { get; set; }

        [Display(Name = "P.O"), Required(ErrorMessage = "Post Office Required")]
        public string MailingPO { get; set; }

        [Display(Name = "Post Code")]
        public int? MailingPostCode { get; set; }

        [Display(Name = "P.S"), Required(ErrorMessage = "Police Station Required")]
        public string MailingPS { get; set; }

        [Display(Name = "District")]
        public int MailingDistrictId { get; set; }


        [Range(1300000000, 1999999999, ErrorMessage = "Input a valid phone number"), Display(Name = "Student's mobile"), Required(ErrorMessage = "Mobile Number Required")]
        public int StudentMobile { get; set; }

        [Range(1300000000, 1999999999, ErrorMessage ="Input a valid phone number"), Display(Name = "Guardian's mobile"), Required(ErrorMessage = "Mobile Number Required")]
        public int GuardianMobile { get; set; }



        [Range(1300000000, 1999999999), Display(Name = "Father's mobile"), Required(ErrorMessage = "Mobile Number Required")]
        public int FatherMobile { get; set; }

        [Range(1300000000, 1999999999), Display(Name = "Mother's mobile"), Required(ErrorMessage = "Mobile Number Required")]
        public int MotherMobile { get; set; }

        [DataType(DataType.Date), Display(Name = "Date Of Birth")]
        public DateTime Birthday { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [StringLength(50)]
        public string Religion { get; set; }

        [Display(Name = "H.S.C Roll"), Required(ErrorMessage = "HSC Roll Required")]
        public int HSCRoll { get; set; }

        [Display(Name = "S.S.C Roll"), Required(ErrorMessage = "SSC Roll Required")]
        public int SSCRoll { get; set; }

        [Required(ErrorMessage = "HSC CGPA Required"), Display(Name = "HSC GPA")]
        public double HSCGPA { get; set; }

        [Required(ErrorMessage = "SSC CGPA Required"), Display(Name = "SSC GPA")]
        public double SSCGPA { get; set; }

        [Display(Name = "College Roll")]
        public int CollegeRoll { get; set; }

        [Display(Name = "Passing Year"), Range(2016, 2019), Required(ErrorMessage = "Passing Year Required")]
        public int SSCPassingYear { get; set; }

        [Display(Name = "Passing Year"), Range(2018, 2021), Required(ErrorMessage = "Passing Year Required")]
        public int HSCPassingYear { get; set; }

        [Display(Name = "SSC Board"), StringLength(20)]
        public string SSCBoard { get; set; }

        [Display(Name = "HSC Board"), StringLength(20)]
        public string HSCBoard { get; set; }
        public string SSCRemark { get; set; }
        public string HSCRemark { get; set; }

        public int NuAdmissionRoll { get; set; }

        public ICollection<SelectListItem> DistrictList { get; set; }
        public ICollection<SelectListItem> SubjectList { get; set; }
    }
}
