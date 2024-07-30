using System.Xml.Serialization;

namespace GT.WebServices.API.Application.Dtos;

[XmlRoot("employee")]
public class EmployeeUpdateDto
{
    [XmlElement("empID")] public string EmployeeId { get; set; }
    [XmlElement("name")] public string Name { get; set; }
    [XmlElement("language")] public string Language { get; set; }
    [XmlElement("roles")] public string Roles { get; set; }
    [XmlElement("verifyBy")] public string VerifyBy { get; set; }
    [XmlElement("keypadID")] public string KeypadId { get; set; }
    [XmlElement("pin")] public string PinCode { get; set; }
    [XmlElement("badgeCode")] public string BadgeCode { get; set; }
    [XmlElement("fingerTemplates")] public BiometricTemplatesDto FingerTemplates { get; set; }
    [XmlElement("faceTemplates")] public BiometricTemplatesDto FaceTemplates { get; set; }
    [XmlElement("photo")] public string Photo { get; set; }
    [XmlElement("time")] public string Timestamp { get; set; }
}

public class BiometricTemplatesDto
{
    [XmlText] public string Value { get; set; }
    [XmlAttribute("reason")] public string Reason { get; set; }
}

public enum ConsentStatus
{
    UserDeleted,
    ConsentDeclined,
    ConsentRenewed,
    Expired,
    ConsentRenewalDeclined
}