using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using OnlineAdmission.Entity;

namespace OnlineAdmission.APP.ViewModels.SMS
{
    public class SMSCreateVM
    {
        public SMSModel SMSModel { get; set; }
        public AppliedStudent AppliedStudent { get; set; }

    }
} 