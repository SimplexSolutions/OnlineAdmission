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
        Task<Student> GetStudentBySSCRollAsync(int hscRoll);
        Task<int> GetCountAsync(int subjectCode);
    }
}
