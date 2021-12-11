using OnlineAdmission.BLL.IManager;
using OnlineAdmission.DAL.IRepository;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.BLL.Manager
{
    public class MeritTypeManager : Manager<MeritType>, IMeritTypeManager
    {
        public MeritTypeManager(IMeritTypeRepository meritTypeRepository) : base(meritTypeRepository)
        {

        }
    }
}
