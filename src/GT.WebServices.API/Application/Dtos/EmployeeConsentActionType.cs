using System;
using System.Collections.Generic;
using System.Text;

namespace GT.WebServices.API.Application.Dtos
{
    public enum EmployeeConsentActionType
    {
        Given,
        Revoked, // Employee Declined Renewal at the terminal
        Expired,
        Declined // Employee Declined Consent at initial prompt at the terminal
    }
}
