using System;
using System.ComponentModel.DataAnnotations;



namespace OnlineAdmission.Entity
{
    public class Student : BaseProps
    {
        [Display(Name = "NU Roll")]
        public int NUAdmissionRoll { get; set; }

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


        [Range(1300000000, 1999999999), Display(Name = "Student's mobile"), Required(ErrorMessage = "Mobile Number Required")]
        public int StudentMobile { get; set; }

        [Range(1300000000, 1999999999), Display(Name = "Guardian's mobile"), Required(ErrorMessage = "Mobile Number Required")]
        public int GuardianMobile { get; set; }

        [Range(1300000000, 1999999999), Display(Name = "Father's mobile"), Required(ErrorMessage = "Mobile Number Required")]
        public int FatherMobile { get; set; }

        [Range(1300000000, 1999999999), Display(Name = "Mother's mobile"), Required(ErrorMessage = "Mobile Number Required")]
        public int MotherMobile { get; set; }

        //[MinimumAge(18)]
        [DataType(DataType.Date), Display(Name = "Date Of Birth")]
        public DateTime Birthday { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [StringLength(50)]
        public string Religion { get; set; }

        [Display(Name = "H.S.C Roll"), Required(ErrorMessage = "HSC Roll Required")]
        public long HSCRoll { get; set; }

        [Display(Name = "S.S.C Roll"), Required(ErrorMessage = "SSC Roll Required")]
        public int SSCRoll { get; set; }

        [Required(ErrorMessage = "HSC CGPA Required"), Display(Name = "HSC GPA")]
        public double HSCGPA { get; set; }

        [Required(ErrorMessage = "SSC CGPA Required"), Display(Name = "SSC GPA")]
        public double SSCGPA { get; set; }

        [Display(Name = "College Roll")]
        public int CollegeRoll { get; set; }

        [Display(Name = "Passing Year"), Required(ErrorMessage = "Passing Year Required")]
        public int SSCPassingYear { get; set; }

        [Display(Name = "Passing Year"), Required(ErrorMessage = "Passing Year Required")]
        public int HSCPassingYear { get; set; }

        [Display(Name = "SSC Board"), StringLength(20)]
        public string SSCBoard { get; set; }

        [Display(Name = "HSC Board"), StringLength(20)]
        public string HSCBoard { get; set; }
        public string SSCRemark { get; set; }
        public string HSCRemark { get; set; }

        
        // 1=Regular Student, 2=SubjectChanged, 3 = CollegeChanged
        public int? StudentType { get; set; } 

        //null=Regular Student
        public int? PreviousCollegeRoll { get; set; }

        //null/0/false =Inactive, 1/true = active
        public bool? Status { get; set; }
        
        //StudentCategory = 1 = hon's Student
        //StudentCategory = 2 = professional Student
        //StudentCategory = 3 = Masters Professional
        //StudentCategory = 4 = MastersGeneral
        public int? StudentCategory { get; set; }

        [Display(Name ="Student NID")]
        [StringLength(maximumLength: 21, MinimumLength = 10)]
        public string StudentNID { get; set; }

        [Display(Name = "Father's NID")]
        [StringLength(maximumLength: 21, MinimumLength = 10)]
        public string FatherNID { get; set; }

        [Display(Name = "Mother's NID")]
        [StringLength(maximumLength:21, MinimumLength =10)]
        public string MotherNID { get; set; }

        //Navigation Properties
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public District PresentDistrict { get; set; }
        public District PermanentDistrict { get; set; }
        public District MailingDistrict { get; set; }
    }
}
