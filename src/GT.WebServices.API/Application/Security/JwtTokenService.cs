using GT.WebServices.API.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GT.WebServices.API.Application.Security
{
   public class JwtTokenService : IJwtTokenService
    {
        private readonly AdsConfigurationOptions _adsConfigurationOptions;

        public JwtTokenService(IOptions<AdsConfigurationOptions> adsConfigurationOptions)
        {
            _adsConfigurationOptions = adsConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(adsConfigurationOptions));
        }

        public string GenerateToken(string serialNumber)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_adsConfigurationOptions.JwtSigningKey));

            if (!double.TryParse(_adsConfigurationOptions.TokenExpiryInMinutes, out var tokenExpiryMinutes)) {
                throw new Exception($"Requires a valid TokenExpiryInMinutes double parameter - failing when attempting to parse '{_adsConfigurationOptions.TokenExpiryInMinutes}'!");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim("serialNumber", serialNumber) }),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
                Issuer = _adsConfigurationOptions.Issuer,
                Audience = _adsConfigurationOptions.Audience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public (bool, SecurityToken) ValidateToken(string token)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_adsConfigurationOptions.JwtSigningKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _adsConfigurationOptions.Issuer,
                    ValidAudience = _adsConfigurationOptions.Audience,
                    IssuerSigningKey = mySecurityKey,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later etc)
                    ClockSkew = TimeSpan.Zero
                }, out validatedToken);
            }
            catch
            {
                return (false, null);
            }
            return (true, validatedToken);
        }
    }
}
