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
    public class NagadManager : INagadManager
    {
        private readonly INagadRepository _nagadRepository;
        public NagadManager(INagadRepository nagadRepository)
        {
            _nagadRepository = nagadRepository;
        }

        public async Task<AppliedStudent> GetAppliedStudentByNURollNagad(int nuRoll)
        {
            return await _nagadRepository.GetAppliedStudentByNURollNagad(nuRoll);
        }

        public async Task<List<AppliedStudent>> GetAppliedStudentsNagad()
        {
            return await _nagadRepository.GetAppliedStudentsNagad();
        }

        public async Task<MeritStudent> GetMeritStudentByNURollNagad(int nuRoll)
        {
            return await _nagadRepository.GetMeritStudentByNURollNagad(nuRoll);
        }

        public async Task<List<MeritStudent>> GetMeritStudentsNagad()
        {
            return await _nagadRepository.GetMeritStudentsNagad();
        }

        public async Task<List<Student>> GetStudentNagad()
        {
            return await _nagadRepository.GetStudentNagad();
        }

        public async Task<Subject> GetSubjectByCodeNagad(int subCode)
        {
            return await _nagadRepository.GetSubjectByCodeNagad(subCode);
        }

        public async Task<List<Subject>> GetSubjectNagad()
        {
            return await _nagadRepository.GetSubjectNagad();
        }
    }
}
