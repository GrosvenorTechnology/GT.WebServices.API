using System.Xml.Serialization;

namespace GT.WebServices.API.Application.Dtos;

[XmlRoot("employees")]
public class EmployeesDto
{
    public EmployeesDto()
    { }

    [XmlElement("employee")] public EmployeeDto[] Employees { get; set; }

    [XmlAttribute("totalEmployeeCount")] public string TotalEmployeeCount { get; set; }
}

/// <summary>
/// empID, revision and name are mandatory and must not be empty when speciﬁed. 
/// </summary>
public class EmployeeDto
{
    public EmployeeDto()
    { }

    [XmlElement("empID")] public string EmployeeId { get; set; }
    [XmlElement("revision")] public DateTimeOffset ModifiedOn { get; set; }
    [XmlElement("name")] public string Name { get; set; }
    [XmlElement("language")] public string Language { get; set; }
    [XmlElement("badgeCode")] public string BadgeCode { get; set; }
    [XmlElement("keypadID")] public string KeypadId { get; set; }
    [XmlElement("pin")] public string PinCode { get; set; }
    [XmlElement("photo")] public string Photo { get; set; }
    [XmlElement("verifyBy")] public string VerifyBy { get; set; }
    [XmlElement("fingerTemplates")] public string FingerTemplates { get; set; }
    [XmlElement("faceTemplates")] public string FaceTemplates { get; set; }
}
