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
    public class StudentManager : Manager<Student>, IStudentManager
    {
        private readonly IStudentRepository _studentRepository;
        public StudentManager(IStudentRepository studentRepository) : base(studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<int> GetCountAsync(int subId)
        {
            return await _studentRepository.GetCountAsync(subId);
        }

        public async Task<Student> GetStudentByHSCRoll(int hscRoll)
        {
            var student = await _studentRepository.GetStudentByHSCRollAsync(hscRoll);
            return student;
        }

        public async Task<Student> GetStudentByHSCRollAsync(int hscRoll)
        {
            return await _studentRepository.GetStudentByHSCRollAsync(hscRoll);
        }

        public async Task<Student> GetStudentBySSCRollAsync(int sscRoll)
        {
            return await _studentRepository.GetStudentBySSCRollAsync(sscRoll);
        }
    }
}
