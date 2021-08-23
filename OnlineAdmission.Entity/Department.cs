using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class Department : BaseProps
    {
        [Required, Display(Name = "Department Name"), StringLength(150)]
        public string DepartmentName { get; set; }

        [Required, Display(Name = "বিভাগের নাম"), StringLength(250)]
        public string DepartmentNameBn { get; set; }

        [Required]
        [StringLength(1)]// C= College, U= University
        public string CollegeOrUniversity { get; set; }
    }
}
