namespace GT.WebServices.API.Biometrics.Models;

[Serializable]
public class Consent : Base<Consent>
{
    public Guid Id { get; set; }
    public string Source { get; set; }
    public DateTime Time { get; set; }
    public DateTime Expiry { get; set; }
    public string Action { get; set; }
    public string Usage { get; set; }
    public string TemplatesHashes { get; set; }
    public string ConsentText { get; set; }

}
