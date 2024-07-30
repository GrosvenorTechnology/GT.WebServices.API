using Microsoft.IdentityModel.Tokens;

namespace GT.WebServices.API.Application.Security;

public interface IJwtTokenService
{
    string GenerateToken(string serialNumber);
    (bool, SecurityToken) ValidateToken(string token);
}