using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels.Student
{
    public class StudentDynamicInfoVM
    {
        public int NuRoll { get; set; }
        public int CategoryId { get; set; }
        public int SessionId { get; set; }
        public MeritType MeritType { get; set; }
        public int PaymentTypeId { get; set; }
        public int StudentPaymentTypeId { get; set; }
        public AppliedStudent AppliedStudent { get; set; }
        public Subject Subject { get; set; }
        public MeritStudent MeritStudent { get; set; }
    }
}
