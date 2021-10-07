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
    public class NagadRepository : INagadRepository
    {
        private readonly OnlineAdmissionDbContext db;
        public NagadRepository(OnlineAdmissionDbContext db)
        {
            this.db = db;
        }


        public async Task<AppliedStudent> GetAppliedStudentByNURollNagad(int nuRoll)
        {
            var applied = await db.AppliedStudents.FirstOrDefaultAsync(a => a.NUAdmissionRoll == nuRoll);
            return applied;
        }

        public async Task<List<AppliedStudent>> GetAppliedStudentsNagad()
        {
            var applied = await db.AppliedStudents.ToListAsync();
            return applied;
        }

        public async Task<MeritStudent> GetMeritStudentByNURollNagad(int nuRoll)
        {
            var merit = await db.MeritStudents.FirstOrDefaultAsync(a => a.NUAdmissionRoll == nuRoll && (a.Comments.Trim().ToLower() == "Quota Merit List".Trim().ToLower()
            || a.PaymentStatus == true || a.NUAdmissionRoll<999999));

            return merit;
        }

        public async Task<List<MeritStudent>> GetMeritStudentsNagad()
        {
            var merit = await db.MeritStudents.ToListAsync();
            return merit;
        }

        public async Task<List<Student>> GetStudentNagad()
        {
            var stu = await db.Students.ToListAsync();
            return stu;
        }

        public async Task<Subject> GetSubjectByCodeNagad(int subCode)
        {
            var sub = await db.Subjects.FirstOrDefaultAsync(a => a.Code == subCode);
            return sub;
        }

        public async Task<List<Subject>> GetSubjectNagad()
        {
            var subject = await db.Subjects.ToListAsync();
            return subject;
        }
    }
}
