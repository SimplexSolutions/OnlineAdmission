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

        public async Task<AppliedStudent> GetByAdmissionRollAsync(int roll)
        {
            return await _appliedStudentRepository.GetByAdmissionRollAsync(roll);
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
