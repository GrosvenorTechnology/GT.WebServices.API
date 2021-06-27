using System;
using System.Collections.Generic;

namespace GT.WebServices.API.Biometrics.Models
{
    [Serializable]
    public class FingerBiometric : Base<FingerBiometric>
    {
        public FingerBiometricTemplate Bio { get; set; }
    }

    public class FingerBiometricTemplate
    {
        public string Ver { get; set; }
        public List<FingerTemplateType> Fingers { get; set; }
        public List<Consent> Consent { get; set; }
    }


    public class FingerTemplateType
    {
        public string FingerCode { get; set; }
        public string TemplateFormat { get; set; }
        public List<FingerTemplates> Templates { get; set; }
    }

    public class FingerTemplates
    {
        public string Template { get; set; }
    }
}