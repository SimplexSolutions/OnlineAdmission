using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DAL.IRepository
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student> GetStudentByHSCRollAsync(int hscRoll);
        Task<Student> GetStudentBySSCRollAsync(int sscRoll);
        Task<int> GetCountAsync(int subId);
    }
}
