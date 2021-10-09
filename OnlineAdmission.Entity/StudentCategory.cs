using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class StudentCategory: BaseProps
    {
        [Display(Name="Category")]
        public string CategoryName { get; set; }
    }
}
