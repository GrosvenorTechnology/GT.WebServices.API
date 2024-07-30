using System.Xml.Serialization;

namespace GT.WebServices.API.Application.Dtos;

[XmlRoot("transaction")]
public class TransactionDto
{
    public TransactionDto()
    { }

    [XmlElement("transID")] public string TransactionId { get; set; }
    [XmlElement("deviceID")] public string DeviceId { get; set; }
    [XmlElement("employee")] public TransactionEmployeeDto Employee { get; set; }
    [XmlElement("data")] public TransactionClockingDto Clocking { get; set; }

    public class TransactionClockingDto
    {
        public TransactionClockingDto()
        { }

        [XmlElement("time")] public string Time { get; set; }
        [XmlElement("type")] public string Type { get; set; }
        // Maybe an enum with 'In' and 'Out'
    }  

    public class TransactionEmployeeDto
    {
        public TransactionEmployeeDto()
        { }

        [XmlElement("empID")] public string Firmware { get; set; }
        [XmlElement("identifiedBy")] public TransactionIdentifiedBy Application { get; set; }
        [XmlElement("verifiedBy")] public TransactionVerifiedBy Employees { get; set; }
    }

    // Can these be enums?
    public class TransactionIdentifiedBy
    {
        public TransactionIdentifiedBy()
        { }

        [XmlElement("keypadID")] public int KeypadID { get; set; }
        [XmlElement("badgeCode")] public int BadgeCode { get; set; }
        [XmlElement("bio")] public string TemplateId { get; set; }
    }

    public class TransactionVerifiedBy
    {
        public TransactionVerifiedBy()
        { }

        [XmlElement("pin")] public int Pin { get; set; }
        [XmlElement("bio")] public int TemplateId { get; set; }
        [XmlElement("none")] public string None { get; set; }
    }
}
