using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.BLL.IManager
{
    public interface IMeritStudentManager : IManager<MeritStudent>
    {
        Task<MeritStudent> GetByAdmissionRollAsync(int NURoll);
        Task<bool> UploadMeritStudentsAsync(List<MeritStudent> meritStudents);
        Task<List<MeritStudent>> GetAppliedStudentAsync(); //Student will filterd with subject Code = 0
        Task<List<MeritStudent>> GetAllWithoutPaidAsync(); //Which students are not paid yet
        Task<List<MeritStudent>> GetSpecialPaymentStudent();
    }
}
