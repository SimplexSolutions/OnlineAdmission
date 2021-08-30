using OnlineAdmission.DAL.IRepository;
using OnlineAdmission.DB;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DAL.Repository
{
    public class SMSRepository : Repository<SMSModel>, ISMSRepository
    {
        public SMSRepository(OnlineAdmissionDbContext context): base(context)
        {

        }
    }
}
