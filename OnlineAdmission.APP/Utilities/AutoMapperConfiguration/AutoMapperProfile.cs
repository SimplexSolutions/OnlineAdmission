using AutoMapper;
using OnlineAdmission.APP.ViewModels;
using OnlineAdmission.APP.ViewModels.AppliedStudents;
using OnlineAdmission.APP.ViewModels.Student;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Utilities.AutoMapperConfiguration
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MeritStudentVM, MeritStudent>();
            CreateMap<MeritStudent, MeritStudentVM>();

            CreateMap<StudentCreateVM, Student>();
            CreateMap<Student, StudentCreateVM>();

            CreateMap<StudentEditVM, Student>();
            CreateMap<Student, StudentEditVM>();

            CreateMap<MeritStudent, AppliedStudentVM>();
            CreateMap<AppliedStudentVM, MeritStudent>();

            CreateMap<Student, StudentCreateVM>();
            CreateMap<StudentCreateVM, Student>();


        }
    }
}
