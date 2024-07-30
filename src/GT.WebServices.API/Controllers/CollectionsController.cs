using System.Xml.Linq;

using GT.WebServices.API.Application.Attributes;
using GT.WebServices.API.Core;
using GT.WebServices.API.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights.DataContracts;
using System.Xml.Serialization;
using System.Xml;

namespace GT.WebServices.API.Controllers;

[ApiController]
[Route("api/collections")]
public class CollectionsController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly TelemetryClient _telemetryClient;
    private readonly ScheduleDataService _scheduleDataService;
    private readonly ITerminalConfiguration _terminalConfiguration;

    public CollectionsController(ILogger<CollectionsController> logger, ITerminalConfiguration terminalConfiguration,
          TelemetryClient telemetryClient, ScheduleDataService scheduleDataService)
    {
        _logger = logger;
        _telemetryClient = telemetryClient;
        _scheduleDataService = scheduleDataService;
        _terminalConfiguration = terminalConfiguration;
    }

    [HttpGet()]
    [CustomJwtAuthorize]
    [ProducesResponseType(typeof(XElement), StatusCodes.Status200OK)]
    public ActionResult GetCollectionIndex(string revision = null)
    {
        RequestTelemetry reqTelemetry = HttpContext?.Features?.Get<RequestTelemetry>();
        reqTelemetry?.Properties?.Add("SerialNumber", _terminalConfiguration.SerialNumber);

        var result = new XElement("collections",
                    new XElement("collection", new XElement("name", "schedules"))
                );

        return Ok(result);
    }

    /// <summary>
    /// Return all current valid Ids for the selected datatype
    /// </summary>
    /// <returns></returns>
    [HttpGet("{datatype}/ids")]
    [CustomJwtAuthorize]
    public ActionResult GetIds([FromRoute] string dataType)
    {
        RequestTelemetry reqTelemetry = HttpContext?.Features?.Get<RequestTelemetry>();
        reqTelemetry?.Properties?.Add("SerialNumber", _terminalConfiguration.SerialNumber);

        if (dataType != "schedules")
        {
            //Demo only supports schedules
            return NotFound();
        }

        var result = new XElement("ids");

        try
        {
            var serialNumber = _terminalConfiguration.SerialNumber;

            var data = _scheduleDataService.GetIds();
            foreach (var id in data)
            {
                result.Add(new XElement("id", id));
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong while calling [{nameof(GetIds)}] for Device with SerialNumber '{_terminalConfiguration?.SerialNumber}");
            // Terminal request cannot fail as the terminal will continue to send the change until accepted.
            return Ok(result);
        }
    }

    [HttpGet("{datatype}")]
    [CustomJwtAuthorize]
    public ActionResult GetData([FromRoute] string dataType, [FromQuery] string revision = null, [FromQuery] int pageSize = 100)
    {
        RequestTelemetry reqTelemetry = HttpContext?.Features?.Get<RequestTelemetry>();
        reqTelemetry?.Properties?.Add("SerialNumber", _terminalConfiguration.SerialNumber);

        var result = new XElement(dataType);

        if (dataType != "schedules")
        {
            //Demo only supports schedules
            result.Add(new XAttribute("totalCount", 0), new XAttribute("unknownDataType", true));
            return NotFound();
        }
                
        try
        {
            var query = _scheduleDataService.Query();
            if (DateTimeOffset.TryParse(revision, out var dtRevision))
            {
                query = query.Where(x => x.LastModifiedOn > dtRevision);
            }

            var data = query.Take(pageSize).ToArray();
            if (data.Length == 0)
            {
                //When all the data has been, let the terminal know how many items there should be.
                result.Add(new XAttribute("totalCount", _scheduleDataService.GetCurrentCount()));
                return Ok(result);
            }

            //Else return a page with data in it

            var seriliser = new XmlSerializer(typeof(Schedule));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            foreach (var schedule in data)
            {
                var node = new XElement("schedule",
                    new XElement("id", schedule.Id),
                    new XElement("externalId", schedule.ExternalId),
                    new XElement("empId", schedule.EmpId),
                    new XElement("name",
                        schedule.Name.Items.Select(x =>
                            new XElement("text", new XAttribute("language", x.Language), x.Value)
                        )),
                    new XElement("startDateTime", schedule.StartDateTime),
                    new XElement("endDateTime", schedule.EndDateTime),
                    new XElement("graceStart", schedule.GraceStart),
                    new XElement("graceEnd", schedule.GraceEnd),
                    new XElement("revision", schedule.LastModifiedOn),
                    new XElement("subSchedules",
                        schedule.SubSchedules.Items.Select(x =>
                            new XElement("subSchedule",
                                new XElement("id", x.Id),
                                new XElement("name",
                                    schedule.Name.Items.Select(x =>
                                        new XElement("text", new XAttribute("language", x.Language), x.Value)
                                    )),
                                new XElement("startDateTime", x.StartDateTime),
                                new XElement("endDateTime", x.EndDateTime)
                            )
                        )
                    ));
                result.Add(node);

            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Something went wrong while calling [{nameof(GetData)}] for Device with SerialNumber '{_terminalConfiguration?.SerialNumber}'!");
            return Problem(detail: ex.Message);
        }
    }





}
