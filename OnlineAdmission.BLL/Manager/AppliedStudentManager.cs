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
    public class AppliedStudentManager : Manager<AppliedStudent>, IAppliedStudentManager
    {
        private readonly IAppliedStudentRepository _appliedStudentRepository;

        public AppliedStudentManager(IAppliedStudentRepository appliedStudentRepository) : base(appliedStudentRepository)
        {
            _appliedStudentRepository = appliedStudentRepository;
        }

        public async Task<AppliedStudent> GetAppliedStudentAsync(int nuRoll, int studentCategroyId, int sessionId)
        {
            return await _appliedStudentRepository.GetAppliedStudentAsync(nuRoll, studentCategroyId, sessionId);
        }

        public async Task<AppliedStudent> GetByAdmissionRollAsync(int roll, int stuCat)
        {
            return await _appliedStudentRepository.GetByAdmissionRollAsync(roll, stuCat);
        }

        public async Task<AppliedStudent> GetByMobileNumber(string mobileNo)
        {
            return await _appliedStudentRepository.GetByMobileNumber(mobileNo.Trim());
        }

        public async Task<bool> UploadAppliedStudentsAsync(List<AppliedStudent> appliedStudents)
        {
            return await _appliedStudentRepository.UploadAppliedStudentsAsync(appliedStudents);
        }
    }
}
