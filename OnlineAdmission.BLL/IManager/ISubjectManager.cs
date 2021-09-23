using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.BLL.IManager
{
    public interface ISubjectManager : IManager<Subject>
    {
        Task<Subject> GetByCodeAsync(int code);
        Task<Subject> GetByStudentIdAsyc(int stuId);
    }
}
