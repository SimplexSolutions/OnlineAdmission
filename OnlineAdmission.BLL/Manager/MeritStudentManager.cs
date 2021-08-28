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

        public async Task<List<MeritStudent>> GetAllWithoutPaidAsync()
        {
            return await _meritStudentRepository.GetAllWithoutPaidAsync();
        }

        public async Task<List<MeritStudent>> GetAppliedStudentAsync()
        {
            return await _meritStudentRepository.GetAppliedStudentAsync();
        }

        public async Task<MeritStudent> GetByAdmissionRollAsync(int NURoll)
        {
            var existStudent = await _meritStudentRepository.GetByAdmissionRollAsync(NURoll);
            return existStudent;
        }

        public IQueryable<MeritStudent> GetMeritStudents()
        {
           return _meritStudentRepository.GetMeritStudents();
        }

        public async Task<List<MeritStudent>> GetSpecialPaymentStudent()
        {
            return await _meritStudentRepository.GetSpecialPaymentStudent();
        }

        public async Task<bool> UploadMeritStudentsAsync(List<MeritStudent> meritStudents)
        {
            return await _meritStudentRepository.UploadMeritStudentsAsync(meritStudents);
        }
    }
}
