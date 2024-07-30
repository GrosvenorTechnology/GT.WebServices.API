using System;
using GT.WebServices.API.Application.Dtos;
using GT.WebServices.API.Domain;
using Riok.Mapperly.Abstractions;

namespace GT.WebServices.API.Application.Mapping
{
    [Mapper]
    public partial class AdsMapper 
    {

        public partial EmployeeDto Map(Employee source);

        [MapperIgnoreSource(nameof(EmployeeDto.FaceTemplates))]
        [MapperIgnoreSource(nameof(EmployeeDto.FingerTemplates))]
        public partial EmployeeUpdateDto MapToUpdateRequest(EmployeeDto source);
    }
}
