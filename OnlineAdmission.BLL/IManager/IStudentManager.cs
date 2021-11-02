using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.BLL.IManager
{
    public interface IStudentManager : IManager<Student>
    {

        Task<Student> GetStudentByHSCRollAsync(int hscRoll);
        Task<Student> GetStudentBySSCRollAsync(int hscRoll, string boardName);
        Task<int> GetCountAsync(int subjectCode);
        Task<Student> GetByAdmissionRollAsync(int NURoll);
        Task<Student> GetByCollegeRollAsync(int CollegeRoll);
        Task<List<Student>> GetStudentsByCategoryAsync(int stuCategory);
        IQueryable<Student> GetStudents();
    }
}
