using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.BLL.IManager
{
    public interface IAppliedStudentManager : IManager<AppliedStudent>
    {
        Task<bool> UploadAppliedStudentsAsync(List<AppliedStudent> appliedStudents);
        Task<AppliedStudent> GetByAdmissionRollAsync(int roll);
        Task<AppliedStudent> GetByMobileNumber(string mobileNo);
    }
}
