using GT.WebServices.API.Application.Dtos;
using GT.WebServices.API.Biometrics.Models;
using Newtonsoft.Json;

namespace GT.WebServices.API.Domain;

public static class EmployeeExtenstions
{
    public static void Update(this Employee employee, EmployeeUpdateDto dto, out bool hasChanged, out List<EmployeeConsentMessageDto> messageList)
    {
        messageList = new List<EmployeeConsentMessageDto>();
        hasChanged = false;

        if (dto.Language != null)
        {
            if (employee.Language != dto.Language)
            {
                employee.Language = dto.Language;
                hasChanged = true;
            }
        }

        if (dto.Roles != null)
        {
            if (employee.Roles != dto.Roles)
            {
                employee.Roles = dto.Roles;
                hasChanged = true;
            }
        }

        if (dto.VerifyBy != null)
        {
            if (employee.VerifyBy != dto.VerifyBy)
            {
                employee.VerifyBy = dto.VerifyBy;
                hasChanged = true;
            }
        }

        if (dto.KeypadId != null)
        {
            if (employee.KeyPadId != dto.KeypadId)
            {
                employee.KeyPadId = dto.KeypadId;
                hasChanged = true;
            }
        }

        if (dto.PinCode != null)
        {
            if (employee.PinCode != dto.PinCode)
            {
                employee.PinCode = dto.PinCode;
                hasChanged = true;
            }
        }

        if (dto.BadgeCode != null)
        {
            if (employee.BadgeCode != dto.BadgeCode)
            {
                employee.BadgeCode = dto.BadgeCode;
                hasChanged = true;
            }
        }

        if (dto.Photo != null)
        {
            if (employee.Photo != dto.Photo)
            {
                employee.Photo = dto.Photo;
                hasChanged = true;
            }
        }

        if (dto.FingerTemplates != null)
        {
            messageList.Add(employee.ProcessTemplate(dto.FingerTemplates, ConsentType.Finger));
            hasChanged = true;
        }

        if (dto.FaceTemplates != null)
        {
            messageList.Add(employee.ProcessTemplate(dto.FaceTemplates, ConsentType.Face));
            hasChanged = true;
        }
    }

    private static EmployeeConsentMessageDto ProcessTemplate(this Employee employee, BiometricTemplatesDto templateDto, ConsentType consentType )
    {
        if (string.IsNullOrWhiteSpace(templateDto.Reason))
        {
            // if template has no reason - consent is add
            // since add and renew code path is same - I am using consentRenewed Reason.
            templateDto.Reason = "consentRenewed";
        }

        var consentStatus = Enum.Parse<ConsentStatus>(templateDto.Reason, true);
        switch (consentStatus)
        {
            case ConsentStatus.ConsentRenewed:
                // add / update bio template
                if (string.IsNullOrWhiteSpace(templateDto.Value))
                {
                    throw new Exception($"Clock-Connect-Api - Unable to renew biometric template. DTO contains no value. {JsonConvert.SerializeObject(templateDto)}");
                }

                employee.SetTemplate(templateDto.Value, consentType);
                break;
            case ConsentStatus.UserDeleted:
            case ConsentStatus.ConsentDeclined:
            case ConsentStatus.Expired:
            case ConsentStatus.ConsentRenewalDeclined:
                // Remove bio template
                employee.SetTemplate(null, consentType);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return BuildMessage(employee.EmployeeId, consentStatus);
    }

    private static void SetTemplate(this Employee employee, string value, ConsentType type)
    {
        switch (type)
        {
            case ConsentType.Face:
                employee.FaceTemplates = value;
                break;
            case ConsentType.Finger:
                employee.FingerTemplates = value;
                break;
            default:
                throw new Exception($"Unknown Consent Type : '{type}'");
        }
    }

    private static EmployeeConsentMessageDto BuildMessage(Guid employeeId, ConsentStatus status)
    {
        EmployeeConsentActionType actionType;
        switch (status)
        {
            case ConsentStatus.UserDeleted:
            case ConsentStatus.ConsentDeclined:
                actionType = EmployeeConsentActionType.Declined;
                break;
            case ConsentStatus.ConsentRenewed:
                actionType = EmployeeConsentActionType.Given;
                break;
            case ConsentStatus.Expired:
                actionType = EmployeeConsentActionType.Expired;
                break;
            case ConsentStatus.ConsentRenewalDeclined:
                actionType = EmployeeConsentActionType.Revoked;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }

        return new EmployeeConsentMessageDto
        {
            Action = actionType,
            EmployeeId = employeeId.ToString()
        };
    }
}
