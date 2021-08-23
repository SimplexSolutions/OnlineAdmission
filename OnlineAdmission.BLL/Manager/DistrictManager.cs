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
    public class DistrictManager : Manager<District>, IDistrictManager
    {
        public DistrictManager(IDistrictRepository districtRepository) : base(districtRepository)
        {

        }
    }
}
