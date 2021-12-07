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
    public class AppliedStudentRepository : Repository<AppliedStudent>, IAppliedStudentRepository
    {
        public AppliedStudentRepository(OnlineAdmissionDbContext onlineAdmissionDbContext) : base(onlineAdmissionDbContext)
        {

        }


        public async Task<AppliedStudent> GetByAdmissionRollAsync(int roll, int stuCat)
        {
            return await _context.AppliedStudents.FirstOrDefaultAsync(a => a.NUAdmissionRoll == roll && a.StudentCategoryId==stuCat);
        }

        public async Task<AppliedStudent> GetByMobileNumber(string mobileNo)
        {
            return await _context.AppliedStudents.FirstOrDefaultAsync(a => a.MobileNo.Trim() == mobileNo.Trim());
        }

        

        public async Task<bool> UploadAppliedStudentsAsync(List<AppliedStudent> appliedStudents)
        {
            await _context.AppliedStudents.AddRangeAsync(appliedStudents);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
