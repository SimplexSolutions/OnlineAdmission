using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DAL.IRepository
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Task<Subject> GetByCodeAsync(int code);
        Task<Subject> GetByStudentIdAsyc(int stuId);
        Task<List<Subject>> GetAllByCategoryIdAsync(int CategoryId);
    }
}
