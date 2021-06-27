using System;

namespace GT.WebServices.API.Application.Dtos
{
    public class EmployeeConsentMessageDto
    {
        public string EmployeeId { get; set; }
        public EmployeeConsentActionType Action { get; set; }
    }
}
