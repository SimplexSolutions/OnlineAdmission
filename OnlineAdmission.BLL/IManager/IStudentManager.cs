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

        Task<Student> GetStudentByHSCRollAsync(long hscRoll);
        Task<Student> GetStudentBySSCRollAsync(int hscRoll, string boardName);
        Task<int> GetCountAsync(int subjectCode);
        Task<Student> GetHonsByAdmissionRollAsync(int NURoll);
        Task<Student> GetByCollegeRollAsync(int CollegeRoll);
        Task<List<Student>> GetStudentsByCategoryAsync(int stuCategory);
        IQueryable<Student> GetStudents();
    }
}
