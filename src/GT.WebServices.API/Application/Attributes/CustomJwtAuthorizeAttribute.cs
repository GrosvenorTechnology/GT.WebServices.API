using GT.WebServices.API.Application.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace GT.WebServices.API.Application.Attributes
{
    public class CustomJwtAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomJwtAuthorizeAttribute()
            : base(typeof(CustomJwtAuthFilter))
        {
            Arguments = new object[] { };
        }
    }
}
