using System.Xml.Serialization;

namespace GT.WebServices.API.Application.Dtos;

[XmlRoot("registration")]
public class DeviceRegistrationDto
{
    public DeviceRegistrationDto()
    { }

    [XmlElement("credentials")] public CredentialsDto Credentials { get; set; }
    [XmlElement("device")] public DeviceDto Device { get; set; }
}

public class CredentialsDto
{
    public CredentialsDto()
    { }

    [XmlElement("username")] public string Username { get; set; }
    [XmlElement("password")] public string Password { get; set; }
}

[XmlRoot("registration")]
public class DeviceTokenDto
{
    public DeviceTokenDto()
    { }

    [XmlElement("token")] public string Token { get; set; }
}
