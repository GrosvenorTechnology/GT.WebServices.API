using System.Text;
using GT.WebServices.API.Biometrics.Helpers;
using Newtonsoft.Json;

namespace GT.WebServices.API.Biometrics.Models;

public abstract class Base<T>
{
    public static string Base64Encode(T obj)
    {
        var jsonString = JsonConvert.SerializeObject(obj,JsonSettings.Serializer);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
    }

    public static T Base64Decode(string base64String)
    {
        var bytes = Convert.FromBase64String(base64String);
        var json = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<T>(json, JsonSettings.Serializer);
    }
}
