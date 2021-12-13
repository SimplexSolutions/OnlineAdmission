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

        public async Task<Student> GetByAdmissionRollAsync(int NURoll, int stuCategory)
        {
            return await _studentRepository.GetByAdmissionRollAsync(NURoll, stuCategory);
        }

        public async Task<Student> GetByCollegeRollAsync(int CollegeRoll)
        {
            return await _studentRepository.GetByCollegeRollAsync(CollegeRoll);
        }

        public async Task<int> GetCountAsync(int subId)
        {
            return await _studentRepository.GetCountAsync(subId);
        }

        public async Task<Student> GetStudentAsync(int nuRoll, int studentCategoryId, int academicSessionId)
        {
            var allStudent = await _studentRepository.GetAllAsync();
            var selectedStudent = allStudent.FirstOrDefault(s => s.NUAdmissionRoll == nuRoll && s.StudentCategoryId == studentCategoryId && s.AcademicSessionId == academicSessionId);
            return selectedStudent;
        }

        public async Task<Student> GetStudentByHSCRoll(long hscRoll)
        {
            var student = await _studentRepository.GetStudentByHSCRollAsync(hscRoll);
            return student;
        }

        public async Task<Student> GetStudentByHSCRollAsync(long hscRoll)
        {
            return await _studentRepository.GetStudentByHSCRollAsync(hscRoll);
        }


        public async Task<Student> GetStudentBySSCRollAsync(int sscRoll, string boardName)
        {
            return await _studentRepository.GetStudentBySSCRollAsync(sscRoll, boardName);
        }

        public IQueryable<Student> GetStudents()
        {
            return _studentRepository.GetStudents();
        }

        public async Task<List<Student>> GetStudentsByCategoryAsync(int stuCategory)
        {
            return await _studentRepository.GetStudentsByCategoryAsync(stuCategory);
        }
    }
}
