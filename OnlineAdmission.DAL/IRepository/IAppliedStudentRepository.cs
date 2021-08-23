using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DAL.IRepository
{
    public interface IAppliedStudentRepository : IRepository<AppliedStudent>
    {
        Task<bool> UploadAppliedStudentsAsync(List<AppliedStudent> appliedStudents);
        Task<AppliedStudent> GetByAdmissionRollAsync(int roll);
    }
}
