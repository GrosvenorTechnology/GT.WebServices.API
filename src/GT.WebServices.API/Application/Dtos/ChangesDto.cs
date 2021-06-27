using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GT.WebServices.API.Application.Dtos
{
    [XmlRoot("changes")]
    public class ChangesDto
    {
        public ChangesDto()
        { }

        [XmlElement("firmware")] public string Firmware { get; set; }
        [XmlElement("application")] public string Application { get; set; }
        [XmlElement("employees")] public string Employees { get; set; }
        [XmlElement("employeeInfo")] public string EmployeeInfo { get; set; }
    }
}
