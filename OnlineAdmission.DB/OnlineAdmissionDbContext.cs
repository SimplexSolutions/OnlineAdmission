using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DB
{
    public class OnlineAdmissionDbContext : IdentityDbContext
    {
        public OnlineAdmissionDbContext(DbContextOptions<OnlineAdmissionDbContext> options): base(options)
        {
        }

        public DbSet<MeritStudent> MeritStudents { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<AppliedStudent> AppliedStudents { get; set; }
        public DbSet<SMSModel> SMSModels { get; set; }
        public DbSet<StudentCategory> StudentCategories { get; set; }
        public DbSet<AcademicSession> AcademicSessions { get; set; }
        public DbSet<MeritType> MeritTypes { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<StudentPaymentType> StudentPaymentTypes { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
