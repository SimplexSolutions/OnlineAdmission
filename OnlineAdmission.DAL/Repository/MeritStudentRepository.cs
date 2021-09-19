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
    public class MeritStudentRepository : Repository<MeritStudent>, IMeritStudentRepository
    {
        
        public MeritStudentRepository(OnlineAdmissionDbContext context) : base(context)
        {
            
        }

        public async Task<List<MeritStudent>> GetAllWithoutPaidAsync()
        {
            return await _context.MeritStudents.Where(m => m.PaymentStatus == false).ToListAsync();
        }

        public async Task<List<MeritStudent>> GetAppliedStudentAsync()
        {
            var appliedStudent = await _context.MeritStudents.Where(s => s.SubjectCode == 0 ).ToListAsync();
            return appliedStudent;
        }

        public async Task<MeritStudent> GetByAdmissionRollAsync(int NURoll)
        {
            var existStudent =  await _context.MeritStudents.FirstOrDefaultAsync(m => m.NUAdmissionRoll == NURoll && m.Comments.Trim().ToLower() == "2nd Merit List".Trim().ToLower());
            return existStudent;
        }

        public IQueryable<MeritStudent> GetMeritStudents()
        {
            IQueryable<MeritStudent> meritStudents =_context.MeritStudents;
            return meritStudents;
        }

        public async Task<List<MeritStudent>> GetSpecialPaymentStudent()
        {
            return await _context.MeritStudents.Where(m => m.DeductedAmaount > 0).ToListAsync();
        }

        public async Task<bool> UploadMeritStudentsAsync(List<MeritStudent> meritStudents)
        {
            await _context.MeritStudents.AddRangeAsync(meritStudents);
            return await _context.SaveChangesAsync()>0;
        }
    }
}
