using System;
using System.Collections.Generic;

namespace GT.WebServices.API.Biometrics.Models
{
    [Serializable]
    public class FaceBiometric : Base<FaceBiometric>
    {
        public FaceBiometricTemplate Face { get; set; }
    }

    public class FaceBiometricTemplate
    {
        public int VerCode { get; set; }
        public FaceTemplateType[] Templates { get; set; }
        public List<Consent> Consent { get; set; }
    }


    public class FaceTemplateType
    {
        public string TemplateFormat { get; set; }
        public string Template { get; set; }
    }
}