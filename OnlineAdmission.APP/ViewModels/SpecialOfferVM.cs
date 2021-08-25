using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels
{
    public class SpecialOfferVM
    {


        public double Amount { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public MeritStudent MeritStudent { get; set; }
        public AppliedStudent AppliedStudent { get; set; }

        public ICollection<SelectListItem> MeritStudentList { get; set; }
    }
}
