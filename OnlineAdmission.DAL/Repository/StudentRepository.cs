using Microsoft.EntityFrameworkCore;
using OnlineAdmission.DAL.IRepository;
using OnlineAdmission.DB;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DAL.Repository
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(OnlineAdmissionDbContext context) : base(context)
        {
        }

        public override async Task<Student> GetByIdAsync(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            return student;   
        }

        public override async Task<List<Student>> GetAllAsync()
        {
            var students = await _context.Students.Include(s => s.Subject).ToListAsync();
            return students;
        }

        public async Task<Student> GetStudentByHSCRollAsync(int hscRoll)
        {
            var student = await _context.Students.Include(s => s.Subject).FirstOrDefaultAsync(s => s.HSCRoll == hscRoll);
            return student;
        }

        public async Task<Student> GetStudentBySSCRollAsync(int sscRoll, string boardName)
        {
            var student = await _context.Students.Include(s => s.Subject).FirstOrDefaultAsync(s => s.SSCRoll == sscRoll && s.SSCBoard.Trim() == boardName.Trim());
            return student;
        }

        public async Task<int> GetCountAsync(int subId)
        {
            var count =await _context.Students.Where(s => s.SubjectId == subId).CountAsync();
            return count;
        }

        public async Task<Student> GetByAdmissionRollAsync(int NURoll)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.NUAdmissionRoll == NURoll);
            return student;
        }

        public async Task<Student> GetByCollegeRollAsync(int CollegeRoll)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.CollegeRoll == CollegeRoll);
            return student;
        }
    }
}
