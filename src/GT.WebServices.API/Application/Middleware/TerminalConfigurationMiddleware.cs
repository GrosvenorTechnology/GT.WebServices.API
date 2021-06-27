using GT.WebServices.API.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GT.WebServices.API.Application.Middleware
{
    public class TerminalConfigurationMiddleware
    {
        private readonly RequestDelegate _next;

        public TerminalConfigurationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ITerminalConfiguration terminalConfiguration)
        {
            if (httpContext.Request.Headers.TryGetValue("X-Gt-Serialnumber", out StringValues serialNumber))
            {
                terminalConfiguration.SerialNumber = serialNumber.SingleOrDefault();
            }
            else
            {
                // NOTE: Here you can throw exception to force client to send the header
                // We're going to handle this in the RegisterDevices endpoint and return a 403 etc accordingly.
            }

            //Move to next delegate/middleware in the pipleline
            await _next.Invoke(httpContext);
        }
    }
}
