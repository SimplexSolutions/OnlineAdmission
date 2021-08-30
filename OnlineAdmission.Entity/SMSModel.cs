using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.Entity
{
    public class SMSModel : BaseProps
    {
        public string Text { get; set; }
        public string MobileList { get; set; }
        public string Description { get; set; }
    }
}
