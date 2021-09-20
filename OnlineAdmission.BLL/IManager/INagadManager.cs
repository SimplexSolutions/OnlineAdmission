using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.BLL.IManager
{
    public interface INagadManager
    {
        Task<List<MeritStudent>> GetMeritStudentsNagad();
        Task<List<AppliedStudent>> GetAppliedStudentsNagad();
        Task<List<Student>> GetStudentNagad();
        Task<List<Subject>> GetSubjectNagad();
        Task<MeritStudent> GetMeritStudentByNURollNagad(int nuRoll);
        Task<AppliedStudent> GetAppliedStudentByNURollNagad(int nuRoll);
        Task<Subject> GetSubjectByCodeNagad(int subCode);
    }
}
