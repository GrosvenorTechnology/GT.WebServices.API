using System;
using AutoMapper;
using GT.WebServices.API.Application.Dtos;
using GT.WebServices.API.Domain;

namespace GT.WebServices.API.Application.Mapping
{
    public class AdsMappingProfile : Profile
    {
        public AdsMappingProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId.ToString()))
                .ReverseMap()
                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => Guid.Parse(s.EmployeeId)))
                ;

            CreateMap<EmployeeDto, EmployeeUpdateDto>();
        }
    }
}
