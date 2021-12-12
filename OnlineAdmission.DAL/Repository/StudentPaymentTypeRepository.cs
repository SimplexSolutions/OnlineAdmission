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
    public class StudentPaymentTypeRepository : Repository<StudentPaymentType>, IStudentPaymentTypeRepository
    {
        public StudentPaymentTypeRepository(OnlineAdmissionDbContext context) : base(context)
        {

        }

        public override async Task<List<StudentPaymentType>> GetAllAsync()
        {
            return await _context.StudentPaymentTypes
                .Include(s => s.AcademicSession)
                .Include(s => s.MeritType)
                .Include(s => s.PaymentType)
                .Include(s => s.StudentCategory)
                .ToListAsync();
        }
        public override async Task<StudentPaymentType> GetByIdAsync(int id)
        {
            return await _context.StudentPaymentTypes
                .Include(s => s.AcademicSession)
                .Include(s => s.MeritType)
                .Include(s => s.PaymentType)
                .Include(s => s.StudentCategory)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
