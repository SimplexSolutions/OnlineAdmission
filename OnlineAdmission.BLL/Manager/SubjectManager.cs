using OnlineAdmission.BLL.IManager;
using OnlineAdmission.DAL.IRepository;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.BLL.Manager
{
    public class SubjectManager : Manager<Subject>, ISubjectManager
    {
        private readonly ISubjectRepository subjectRepository;
        public SubjectManager(ISubjectRepository context) : base(context)
        {
            subjectRepository = context;
        }

        public async Task<Subject> GetByCodeAsync(int code)
        {
            return await subjectRepository.GetByCodeAsync(code);
        }
    }
}
