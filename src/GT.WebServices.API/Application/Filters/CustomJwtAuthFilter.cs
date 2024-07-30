using GT.WebServices.API.Application.Security;
using GT.WebServices.API.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

namespace GT.WebServices.API.Application.Filters;

public class CustomJwtAuthFilter : IAuthorizationFilter
{
    public CustomJwtAuthFilter()
    { }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        try
        {
            var terminalConfiguration = context.HttpContext.RequestServices.GetRequiredService<ITerminalConfiguration>();

            // Note: this is not a standard w3 bearer nor basic auth implementation - rather a custom blend of the 2 to support the terminal code.
            string authHeader = context.HttpContext.Request.Headers["Authorization"];
            if (authHeader != null && !string.IsNullOrWhiteSpace(terminalConfiguration.SerialNumber))
            {
                var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                if (authHeaderValue.Scheme == "Basic")
                {
                    var jwtTokenService = context.HttpContext.RequestServices.GetRequiredService<IJwtTokenService>();

                    var credentials = Encoding.UTF8
                                       .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))
                                       .Split(':', 2);
                    if (credentials.Length == 2)
                    {
                        // Note: the terminal sends this as the following format into <deviceId:authToken>
                        // HOWEVER, we no longer use the deviceId but instead require the SerialNumber which is sent in a header
                        var valitateResult = jwtTokenService.ValidateToken(credentials[1]);
                        if (valitateResult.Item1)
                        {
                            var jwtToken = (JwtSecurityToken)valitateResult.Item2;
                            var serialNumber = jwtToken.Claims.First(x => x.Type == "serialNumber").Value;

                            // Finally ensure that this token is scoped to the correct device
                            if (terminalConfiguration.SerialNumber == serialNumber)
                            {
                                // attach to context on successful jwt validation for any future endeavours
                                context.HttpContext.Items["serialNumber"] = serialNumber;
                                return;
                            }
                        }
                    }
                }
            }

            ReturnUnauthorizedResult(context);
        }
        catch (FormatException)
        {
            ReturnUnauthorizedResult(context);
        }
    }

    private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
    {
        context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";
        context.Result = new UnauthorizedResult();
    }
}
