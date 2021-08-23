using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class InstituteInfo
    {
        public int Id { get; set; }
        [Display(Name = "Institute Name"), Required]
        public string CollegeName { get; set; }
        [Display(Name = "শিক্ষা প্রতিষ্ঠান নাম"), Required]
        public string CollegeNameBn { get; set; }
        [Display(Name = "Institute Code"), Required]
        public string CollegeCode { get; set; }
        [Display(Name = "Institute Logo"), Required]
        public string CollegeLogo { get; set; }
        [Display(Name = "Institute Slogan"), Required]
        public string CollegeSlogan { get; set; }
        [Display(Name = "Institute Banner"), Required]
        public string CollegeBanner { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Fax { get; set; }
        [Range(01300000000, 01900000000)]
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string WebsiteLink { get; set; }
        [Required]
        public string Address { get; set; }
        [Display(Name = "Head Of Institute Name"), Required]
        public string HeadOfInstituteName { get; set; }
        [Required]
        public string Founder { get; set; }
        [Display(Name = "Year Of Establishment"), Required]
        public string YearOfEstablishment { get; set; }
        [Display(Name = "College Type"), Required]
        public string CollegeType { get; set; }
        [Required]
        public string District { get; set; }
    }
}
