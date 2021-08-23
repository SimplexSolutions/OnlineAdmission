using AutoMapper;
using OnlineAdmission.API.ViewModels.MeritStudent;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.API.Utilities.AutoMapperConfiguration
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MeritStudentCreateVM, MeritStudent>();
            CreateMap<MeritStudent, MeritStudentCreateVM>();

            CreateMap<MeritStudentEditVM, MeritStudent>();
            CreateMap<MeritStudent, MeritStudentEditVM>();
        }
    }
}
