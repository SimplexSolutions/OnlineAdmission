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
    public class StudentPaymentTypeRepository : Repository<StudentPaymentType>, IStudentPaymentTypeRepository
    {
        public StudentPaymentTypeRepository(OnlineAdmissionDbContext context) : base(context)
        {

        }
    }
}
