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
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        public SubjectRepository(OnlineAdmissionDbContext context) : base(context)
        {

        }

        public override async Task<List<Subject>> GetAllAsync()
        {
           return await _context.Subjects.OrderBy(s => s.SubjectName).ToListAsync();
        }

        public async Task<List<Subject>> GetAllByCategoryIdAsync(int typeId)
        {
            return await _context.Subjects.Where(s => s.StudentCategoryId == typeId).OrderBy(s => s.SubjectName).ToListAsync();
        }

        public async Task<Subject> GetByCodeAsync(int code)
        {
            return await _context.Subjects.FirstOrDefaultAsync(s => s.Code == code);
        }

        public async Task<Subject> GetByStudentIdAsyc(int stuId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == stuId);
            var Subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == student.SubjectId);
            return Subject;
        }
    }
}
