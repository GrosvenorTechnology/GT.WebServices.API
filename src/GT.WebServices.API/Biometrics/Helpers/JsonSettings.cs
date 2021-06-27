using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace GT.WebServices.API.Biometrics.Helpers
{
    public static class JsonSettings
    {
        private const string DateTimeFormat = "yyyy-MM-ddThh:mm:ss";
        private static readonly IsoDateTimeConverter DateTimeConverter = new IsoDateTimeConverter{ DateTimeFormat = DateTimeFormat};

        public static readonly JsonSerializerSettings Serializer = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = {DateTimeConverter},
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
