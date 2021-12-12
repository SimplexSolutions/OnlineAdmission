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
    public class MeritStudentManager : Manager<MeritStudent>, IMeritStudentManager
    {
        private readonly IMeritStudentRepository _meritStudentRepository;

        public MeritStudentManager(IMeritStudentRepository meritStudentRepository) : base(meritStudentRepository)
        {
            _meritStudentRepository = meritStudentRepository;
        }


        public async Task<MeritStudent> GetByAdmissionRollAsync(int NURoll, int categoryId, string comments)
        {
            return await _meritStudentRepository.GetByAdmissionRollAsync(NURoll, categoryId, comments);
        }
        public async Task<List<MeritStudent>> GetAllWithoutPaidAsync()
        {
            return await _meritStudentRepository.GetAllWithoutPaidAsync();
        }

        public async Task<List<MeritStudent>> GetAppliedStudentAsync()
        {
            return await _meritStudentRepository.GetAppliedStudentAsync();
        }

        public async Task<MeritStudent> GetHonsByAdmissionRollAsync(int NURoll)
        {
            var existStudent = await _meritStudentRepository.GetHonsByAdmissionRollAsync(NURoll);
            
            return existStudent;
        }

        public async Task<MeritStudent> GetDegreeByAdmissionRollAsync(int NuRoll)
        {
            return await _meritStudentRepository.GetDegreeByAdmissionRollAsync(NuRoll);
        }

        public async Task<MeritStudent> GetGenMastersByAdmissionRollAsync(int NuRoll)
        {
            return await _meritStudentRepository.GetGenMastersByAdmissionRollAsync(NuRoll);
        }

        public IQueryable<MeritStudent> GetMeritStudents()
        {
           return _meritStudentRepository.GetMeritStudents();
        }

        public IQueryable<MeritStudent> GetMeritStudentsByCategory(int cat)
        {
            return _meritStudentRepository.GetMeritStudentsByCategory(cat);
        }

        public Task<MeritStudent> GetProByAdmissionRollAsync(int NuRoll)
        {
            return _meritStudentRepository.GetProByAdmissionRollAsync(NuRoll);
        }

        public Task<MeritStudent> GetProMBAByAdmissionRollAsync(int NuRoll)
        {
            return _meritStudentRepository.GetProMBAByAdmissionRollAsync(NuRoll);
        }

        public async Task<List<MeritStudent>> GetSpecialPaymentStudent()
        {
            return await _meritStudentRepository.GetSpecialPaymentStudent();
        }

        public async Task<bool> UploadMeritStudentsAsync(List<MeritStudent> meritStudents)
        {
            return await _meritStudentRepository.UploadMeritStudentsAsync(meritStudents);
        }

        public async Task<MeritStudent> GetMeritStudentAsync(int nuRoll, int studentCategoryId, int meritTypeId, int sessionId)
        {
            return await _meritStudentRepository.GetMeritStudentAsync(nuRoll, studentCategoryId, meritTypeId, sessionId);
        }
    }
}
