﻿using GT.WebServices.API.Application.Attributes;
using GT.WebServices.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GT.WebServices.API.Controllers;

[ApiController]
[Route("api")]
public class ConfigController : ControllerBase
{
    private readonly IDataCollectionService _dcService;

    public ConfigController(IDataCollectionService dcService)
    {
        _dcService = dcService;
    }

    [HttpGet("devices/{deviceID}/config/{resource}")]
    [CustomJwtAuthorize]
    public ActionResult<string> GetConfigResource(string deviceID, string resource)
    {
        if (resource == "datacollection.xml")
        {
            Response.Headers.Append("X-Revision", _dcService.GetRevision().ToString("o"));
            return new ObjectResult(_dcService.GetData());
        }
        return NotFound();
    }
}
