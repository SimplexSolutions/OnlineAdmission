using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class District : BaseProps
    {
        [Required, Display(Name = "District Name"), StringLength(100)]
        public string DistrictName { get; set; }

        [Required, Display(Name = "জেলার নাম"), StringLength(150)]
        public string DistrictNameBn { get; set; }
    }
}
