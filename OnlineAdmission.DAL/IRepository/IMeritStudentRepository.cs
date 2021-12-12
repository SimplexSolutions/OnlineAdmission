using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DAL.IRepository
{
    public interface IMeritStudentRepository : IRepository<MeritStudent>
    {
        Task<MeritStudent> GetByAdmissionRollAsync(int NURoll, int categoryId, string comments); //ALL student categroy
        Task<MeritStudent> GetHonsByAdmissionRollAsync(int NURoll); //student categroy =1
        Task<MeritStudent> GetProMBAByAdmissionRollAsync(int NuRoll); //student categroy =3
        Task<MeritStudent> GetProByAdmissionRollAsync(int NuRoll); //student categroy = 2
        Task<MeritStudent> GetGenMastersByAdmissionRollAsync(int NuRoll); //student categroy = 4
        Task<MeritStudent> GetDegreeByAdmissionRollAsync(int NuRoll); //student categroy = 5
        Task<MeritStudent> GetMeritStudentAsync(int nuRoll, int studentCategoryId, int meritTypeId, int sessionId);

        Task<bool> UploadMeritStudentsAsync(List<MeritStudent> meritStudents);
        Task<List<MeritStudent>> GetAppliedStudentAsync(); //Student will filterd with subject Code=0
        Task<List<MeritStudent>> GetAllWithoutPaidAsync(); //which students didn't admitted yet
        Task<List<MeritStudent>> GetSpecialPaymentStudent();
        IQueryable<MeritStudent> GetMeritStudents();
        IQueryable<MeritStudent> GetMeritStudentsByCategory(int cat);
    }
}
