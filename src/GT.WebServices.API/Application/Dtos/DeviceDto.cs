using System.Xml.Serialization;

namespace GT.WebServices.API.Application.Dtos
{
   [XmlRoot("device")]
    public class DeviceDto
    {
        public DeviceDto()
        { }

        [XmlElement("deviceID")] public string DeviceId { get; set; }
        [XmlElement("deviceType")] public string DeviceType { get; set; }
        [XmlElement("macAddress")] public string MacAddress { get; set; }
        [XmlElement("ipAddress")] public string IpAddress { get; set; }
        [XmlElement("firmware")] public string Firmware { get; set; }
        [XmlElement("hardware")] public HardwareDto Hardware { get; set; }
        [XmlElement("application")] public ApplicationDto Application { get; set; }
    }

    public class HardwareDto
    {
        [XmlElement("reader")] public HardwareTypeDto Reader { get; set; }
        [XmlElement("biometric")] public HardwareTypeDto Biometric { get; set; }
    }

    public class HardwareTypeDto
    {
        [XmlElement("type")] public string Type { get; set; }
    }

    public class ApplicationDto
    {
        [XmlElement("name")] public string Name { get; set; }
        [XmlElement("version")] public string Version { get; set; }
    }
}
