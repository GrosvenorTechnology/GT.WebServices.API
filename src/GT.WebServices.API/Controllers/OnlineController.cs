using GT.WebServices.API.Application.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace GT.WebServices.API.Controllers;

[ApiController]
[Route("api")]
[CustomJwtAuthorize]
public class OnlineController : ControllerBase
{
    [HttpGet("onlinestate")]
    public ActionResult GetOnlineState()
    {
        var result = new XElement("result");
        result.Add(new XElement("isOnline", "true"));
        return Ok(result);
    }
}
