using GT.WebServices.API.Application.Filters;
using Microsoft.AspNetCore.Mvc;

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
