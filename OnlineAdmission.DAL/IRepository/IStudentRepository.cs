using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DAL.IRepository
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student> GetStudentByHSCRollAsync(long hscRoll);
        Task<Student> GetStudentBySSCRollAsync(int sscRoll, string boardName);
        Task<int> GetCountAsync(int subId);
        Task<Student> GetByAdmissionRollAsync(int NURoll, int stuCategory);
        Task<Student> GetByCollegeRollAsync(int CollegeRoll);
        Task<List<Student>> GetStudentsByCategoryAsync(int stuCategory);
        IQueryable<Student> GetStudents();
        Task<Student> GetStudentAsync(int nuRoll, int studentCategoryId, int academicSessionId);

    }
}
