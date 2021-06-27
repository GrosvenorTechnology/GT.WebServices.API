
namespace GT.WebServices.API.Core
{
    public class AdsConfigurationOptions
    {
        public const string Name = nameof(AdsConfigurationOptions);

        public string JwtSigningKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string TokenExpiryInMinutes { get; set; }
    }
}
