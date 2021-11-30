﻿using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DAL.IRepository
{
    public interface IMeritStudentRepository : IRepository<MeritStudent>
    {
        Task<MeritStudent> GetByAdmissionRollAsync(int NURoll);
        Task<MeritStudent> GetProMBAByAdmissionRollAsync(int NuRoll);
        Task<MeritStudent> GetProByAdmissionRollAsync(int NuRoll);
        Task<MeritStudent> GetGenMastersByAdmissionRollAsync(int NuRoll);

        Task<bool> UploadMeritStudentsAsync(List<MeritStudent> meritStudents);
        Task<List<MeritStudent>> GetAppliedStudentAsync(); //Student will filterd with subject Code=0
        Task<List<MeritStudent>> GetAllWithoutPaidAsync(); //which students didn't admitted yet
        Task<List<MeritStudent>> GetSpecialPaymentStudent();
        IQueryable<MeritStudent> GetMeritStudents();
        IQueryable<MeritStudent> GetMeritStudentsByCategory(int cat);
    }
}
