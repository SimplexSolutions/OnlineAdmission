using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.ViewModels
{
    public class MeritStudentVM
    {
        public int Id { get; set; }
        public int NUAdmissionRoll { get; set; }
        public string ApplicantName { get; set; }
        public long HSCRoll { get; set; }
        public int SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string HSCGroup { get; set; }
        public int MeritPosition { get; set; }
        public string Comments { get; set; }
        public bool PaymentStatus { get; set; }
        public double PaidAmaount { get; set; }
    }
}
