using GT.WebServices.API.Biometrics.Models;
using GT.WebServices.API.Domain;

namespace GT.WebServices.API.Biometrics;

public static class Extensions
{
    /// <summary>
    /// Processes employees Face and Finger templates. Removes expired templates.
    /// Returns true if the object has been updated and changes need to be synced.
    /// </summary>
    /// <param name="employee">Employee object</param>
    /// <returns>true if there were changes made to the object</returns>
    public static bool RemoveExpiredTemplates(this Employee employee)
    {
        var faceIsExpired = false;
        var fingerIsExpired = false;

        if (!string.IsNullOrWhiteSpace(employee.FaceTemplates))
        {
            var biometric = FaceBiometric.Base64Decode(employee.FaceTemplates);
            faceIsExpired = biometric.Face.Consent.Any(consent => consent.Expiry < DateTime.UtcNow);
            if (faceIsExpired)
            {
                employee.FaceTemplates = null;
                employee.ModifiedOn = DateTime.UtcNow;
            }

        }

        if (!string.IsNullOrWhiteSpace(employee.FingerTemplates))
        {
            var biometric = FingerBiometric.Base64Decode(employee.FingerTemplates);
            fingerIsExpired = biometric.Bio.Consent.Any(consent => consent.Expiry < DateTime.UtcNow);
            if (fingerIsExpired)
            {
                employee.FingerTemplates = null;
                employee.ModifiedOn = DateTime.UtcNow;
            }
        }


        return faceIsExpired | fingerIsExpired;
    }

    public static int GetTemplateCount(this Employee employee)
    {
        var count = 0;
        if (!string.IsNullOrWhiteSpace(employee.FaceTemplates))
        {
            var bio = FaceBiometric.Base64Decode(employee.FaceTemplates);
            count += bio.Face.Templates.Count();

        }

        if (!string.IsNullOrWhiteSpace(employee.FingerTemplates))
        {
            var bio = FingerBiometric.Base64Decode(employee.FingerTemplates);
            count += bio.Bio.Fingers.Count();
        }

        return count;
    }
}
