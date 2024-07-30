using System.Diagnostics;
using System.Xml.Linq;

using GT.WebServices.API.Application.Attributes;
using GT.WebServices.API.Application.Dtos;
using GT.WebServices.API.Application.Security;
using GT.WebServices.API.Core;
using GT.WebServices.API.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Microsoft.ApplicationInsights.DataContracts;
using GT.WebServices.API.Domain;

namespace GT.WebServices.API.Controllers;

[ApiController]
[Route("api")]
public class DataController : ControllerBase
{
    private readonly ILogger<DataController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly TelemetryClient _telemetryClient;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITerminalConfiguration _terminalConfiguration;
    private readonly EmployeeDataService _employeeDataService;
    private readonly IDataCollectionService _dataCollectionService;
    private readonly JobCategoryDataService _jobCategoryDataService;
    private readonly JobCodeDataService _jobCodeDataService;

    public DataController(ILogger<DataController> logger, IWebHostEnvironment webHostEnvironment, ITerminalConfiguration terminalConfiguration,
          TelemetryClient telemetryClient, IConfiguration configuration, IJwtTokenService jwtTokenService,
          EmployeeDataService employeeDataService, IDataCollectionService dataCollectionService,
          JobCategoryDataService jobCategoryDataService, JobCodeDataService jobCodeDataService)
    {
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
        _telemetryClient = telemetryClient;
        _jwtTokenService = jwtTokenService;
        _terminalConfiguration = terminalConfiguration;
        _employeeDataService = employeeDataService;
        _dataCollectionService = dataCollectionService;
        _jobCategoryDataService = jobCategoryDataService;
        _jobCodeDataService = jobCodeDataService;
    }

    [HttpGet("changes/{deviceID}")]
    [CustomJwtAuthorize]
    public ActionResult GetChanges(
        string deviceID,
        string empsRevision = null, int? empsCount = null,
        string jobCodesRevision = null, int? jobCodesCount = null,
        string jobCategoriesRevision = null, int? jobCategoriesCount = null,
        string schedulesRevision = null, int? schedulesCount = null,
        string dataCollectionRevision = null
    )
    {
        bool hasEmployeeChanges(string serialNumber)
        {
            if (empsRevision == null) return true;

            var query = _employeeDataService.EmployeesQuery(serialNumber);

            var empsAfterRevision = false;
            var empsCountsDontMatch = false;

            if (empsCount != null)
            {
                var empsCountMatches = query.Count(x => !x.IsDeleted) == empsCount;
                empsCountsDontMatch = !empsCountMatches;
            }

            if (empsCountsDontMatch) return true;

            if (empsRevision != null)
            {
                var revision = DateTimeOffset.Parse(empsRevision);
                empsAfterRevision = query.Any(x => x.ModifiedOn > revision);
            }

            return empsAfterRevision || empsCountsDontMatch;
        }

        bool hasJobCategoriesChanges(string serialNumber)
        {
            if (jobCategoriesRevision == null) return true;

            var query = _jobCategoryDataService.Query();

            var itemsAfterRevision = false;
            var itemCountsDontMatch = false;

            if (jobCategoriesCount != null)
            {
                itemCountsDontMatch = query.Count() != jobCategoriesCount ;
            }

            if (itemCountsDontMatch) return true;

            if (jobCategoriesRevision != null)
            {
                var revision = DateTimeOffset.Parse(jobCategoriesRevision);
                itemsAfterRevision = query.Any(x => x.LastModifiedOn > revision);
            }

            return itemsAfterRevision || itemCountsDontMatch;
        }

        bool hasJobCodeChanges(string serialNumber)
        {
            if (jobCodesRevision == null) return true;

            var query = _jobCodeDataService.Query();

            var itemsAfterRevision = false;
            var itemCountsDontMatch = false;

            if (jobCodesCount != null)
            {
                itemCountsDontMatch = query.Count() != jobCodesCount;
            }

            if (itemCountsDontMatch) return true;

            if (jobCodesRevision != null)
            {
                var revision = DateTimeOffset.Parse(jobCodesRevision);
                itemsAfterRevision = query.Any(x => x.LastModifiedOn > revision);
            }

            return itemsAfterRevision || itemCountsDontMatch;
        }

        var changes = new XElement("changes");

        try
        {
            var serialNumber = _terminalConfiguration.SerialNumber;

            if (hasEmployeeChanges(serialNumber)) changes.Add(new XElement("employees"));
            if (hasJobCategoriesChanges(serialNumber)) changes.Add(new XElement("jobCategories"));
            if (hasJobCodeChanges(serialNumber)) changes.Add(new XElement("jobCodes"));

            //Add other resources here, such as DataCollection
            //if (DateTime.TryParse(dataCollectionRevision, out var revision))
            //{ 
            //    if (revision < _dataCollectionService.GetRevision())
            //    {
            //        changes.Add(new XElement("dataCollection"));
            //    }
            //}
            //else
            //{
            //    changes.Add(new XElement("dataCollection"));
            //}


            return Ok(changes);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Something went wrong while calling [{nameof(GetChanges)}] for Device with SerialNumber '{_terminalConfiguration?.SerialNumber}'!\r\n{ex.Message}");
            return Ok(changes);
        }
    }

    /// <summary>
    /// The revision can be empty during the intial synchronization from the terminal
    /// </summary>
    /// <remarks>All endpoints must return 200 OK!</remarks>
    /// <param name="revision"></param>
    /// <returns></returns>
    [HttpGet("employees")]
    [CustomJwtAuthorize]
    public ActionResult GetEmployees(string revision = null)
    {
        var result = new XElement("employees");

        var operation = _telemetryClient.StartOperation<DependencyTelemetry>(nameof(GetEmployees));
        operation.Telemetry.Id = $"{nameof(GetEmployees)}_{Guid.NewGuid()}";
        operation.Telemetry.Type = "Processing";
        operation.Telemetry.Data = $"SerialNumber_{_terminalConfiguration?.SerialNumber}";

        var success = false;
        var start = DateTime.UtcNow;
        var sw = new Stopwatch();
        sw.Start();

        try
        {
            var serialNumber = _terminalConfiguration.SerialNumber;
            //Validate that device is valid here

            //We use timestamps for revisions so they are sortable and work with last modified of resource records
            DateTimeOffset? revisionDate = null;
            try
            {
                if (revision != null)
                {
                    revisionDate = DateTimeOffset.Parse(revision);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Revision '{revision}'is not a valid datetime value!");
                return Ok(result);
            }

            //Access All Employees
            var query = _employeeDataService.EmployeesQuery(serialNumber);

            var revisionEmployees = new List<Employee>();
            if (revisionDate != null)
            {
                //Only get changes post revision date
                revisionEmployees = query
                    .Where(x => x.ModifiedOn > revisionDate.Value.UtcDateTime)
                    .OrderBy(x => x.ModifiedOn).ToList();
            }
            else
            {
                //Get them all (but no deletes, clock will use ID list to delete old)
                revisionEmployees = query.Where(x => !x.IsDeleted).ToList(); ;
            }

            var combined = revisionEmployees.Select(x => x.EmployeeId.ToString()).ToList();

            // "An empty employees tag must be returned if no employees were added or modiﬁed".
            // So omit the total attribute accordingly.
            if (revisionEmployees.Count() > 0)
            {
                var totalEmployees = query.Where(x => !x.IsDeleted).Count();
                result.Add(new XAttribute("totalEmployeeCount", totalEmployees.ToString()));
            }

            foreach (var employeeId in combined)
            {
                var employee = revisionEmployees.SingleOrDefault(x => x.EmployeeId.ToString() == employeeId);

                if (employee == null || employee.IsDeleted)
                {
                    // TODO: Since we don't know when the employee was unlinked, What is the correct revision value to pass back? DateTime.UtcNow?!
                    result.Add(new XElement("employee",
                        new XElement("empID", employeeId),
                        new XElement("revision", employee?.ModifiedOn ?? DateTime.UtcNow)
                    ));
                }
                else
                {
                    result.Add(new XElement("employee",
                        new XElement("empID", employee.EmployeeId.ToString()),
                        new XElement("revision", employee.ModifiedOn),
                        new XElement("name", employee.Name),
                        new XElement("language", employee.Language),
                        new XElement("roles", employee.Roles),
                        new XElement("badgeCode", employee.BadgeCode),
                        new XElement("keypadID", employee.KeyPadId),
                        new XElement("pin", employee.PinCode),
                        new XElement("photo", employee.Photo),
                        new XElement("verifyBy", employee.VerifyBy),
                        new XElement("fingerTemplates", employee.FingerTemplates),
                        new XElement("faceTemplates", employee.FaceTemplates)
                        )
                    );
                }
            }

            success = true;
            return Ok(result);
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackException(ex);
            _logger.LogWarning($"Something went wrong while calling [{nameof(GetEmployees)}] for Device with SerialNumber '{_terminalConfiguration?.SerialNumber}'!\r\n{ex.Message}");
            return Ok(result);
        }
        finally
        {
            _telemetryClient.TrackDependency("GetEmployeesProcessing", "GetEmployeesProcessing", "SerialNumber_{serialNumber}", start, sw.Elapsed, success);
            _telemetryClient.StopOperation(operation);
        }
    }

    /// <summary>
    /// Consumes, validates and persists changes from the terminal. These changes will be pushed down to the terminal
    /// in confirmation the next time it polls in.
    /// </summary>
    /// <remarks>All endpoints must not fail or the terminal will get stuck in a loop!</remarks>
    /// <param name="empID"></param>
    /// <param name="employeeUpdateDto"></param>
    /// <returns></returns>
    [HttpPatch("employees/{empID}")]
    [CustomJwtAuthorize]
    public async Task<ActionResult<string>> UpdateEmployee(string empID, [FromBody] EmployeeUpdateDto employeeUpdateDto)
    {
        var serialNumber = _terminalConfiguration.SerialNumber;

        try
        {
            // Handles when junk is sent in on the empId.
            //
            if (!Guid.TryParse(empID, out var employeeId))
            {
                _logger.LogError($"Employee not found. EmployeeId: {empID} | employeeUpdateDto: {JsonConvert.SerializeObject(employeeUpdateDto)}");
                return Ok($"Employee empID '{empID}' not found!");
            }

            var employee = _employeeDataService.EmployeesQuery(serialNumber).FirstOrDefault(x => x.EmployeeId == employeeId);
            if (employee == null || employeeId != Guid.Parse(employeeUpdateDto.EmployeeId))
            {
                _logger.LogError($"Employee not found. EmployeeId: {empID} | employeeUpdateDto: {JsonConvert.SerializeObject(employeeUpdateDto)}");
                return Ok($"Employee empID '{empID}' not found!");
            }

            try
            {
                _logger.LogTrace($"Updating employee empID '{empID}'");
                employee.Update(employeeUpdateDto, out var hasChanged, out var messageList);

                if (hasChanged)
                {
                    employee.ModifiedOn = DateTime.UtcNow;
                    await _employeeDataService.Update(employee);

                    foreach (var consentDto in messageList)
                    {
                        //Do something with consent messages
                    }
                }
            }
            catch (Exception ex)
            {
                var message = $"Failed to update employee empID '{empID}'!.\r\n{ex.Message}";
                _logger.LogError(ex, message);
                return Ok(message);
            }

            _logger.LogTrace($"Employee empID '{empID}' has been updated successfully.");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong while calling [{nameof(UpdateEmployee)}] and trying to update Employee '{empID}' for Device with SerialNumber '{_terminalConfiguration?.SerialNumber}'! | {JsonConvert.SerializeObject(employeeUpdateDto)}\r\n{ex.Message}");
            // Terminal request cannot fail as the terminal will continue to send the change until accepted.
            return Ok();
        }
    }

    /// <summary>
    /// Return all current valid EmployeeIds
    /// </summary>
    /// <returns></returns>
    [HttpGet("employeeIDs")]
    [CustomJwtAuthorize]
    public ActionResult GetEmployeeIds()
    {
        var result = new XElement("employeeIDs");

        try
        {
            var serialNumber = _terminalConfiguration.SerialNumber;

            var employees = _employeeDataService.GetCurrentEmployeeIds(serialNumber);
            foreach (var employeeId in employees)
            {
                result.Add(new XElement("empID", employeeId));
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Something went wrong while calling [{nameof(GetEmployeeIds)}] for Device with SerialNumber '{_terminalConfiguration?.SerialNumber}'!\r\n{ex.Message}");
            // Terminal request cannot fail as the terminal will continue to send the change until accepted.
            return Ok(result);
        }
    }

    [HttpGet("jobCategories")]
    [CustomJwtAuthorize]
    [ProducesResponseType(typeof(XElement), StatusCodes.Status200OK)]
    public ActionResult GetJobCategories(string revision = null)
    {
        RequestTelemetry reqTelemetry = HttpContext?.Features?.Get<RequestTelemetry>();
        reqTelemetry?.Properties?.Add("SerialNumber", _terminalConfiguration.SerialNumber);

        try
        {
            var result = new XElement("jobCategories");
            
            var data = _jobCategoryDataService.Query().ToList();

            var highestRevision = DateTimeOffset.MinValue;

            foreach (var jobCategory in data)
            {
                highestRevision = jobCategory.LastModifiedOn;

                result.Add(new XElement("jobCategory",
                    new XElement("id", jobCategory.Id),
                    new XElement("level", jobCategory.Order),
                    new XElement("name", jobCategory.Name)
                    ));
            }

            if (highestRevision != DateTimeOffset.MinValue)
            {
                //Only put this in if there's a row
                result.AddFirst(new XElement("revision", highestRevision.ToString("O")));
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Something went wrong while calling [{nameof(GetJobCategories)}] for Device with SerialNumber '{_terminalConfiguration?.SerialNumber}'!");
            return Problem(detail: ex.Message);
        }
    }


    [HttpGet("jobCodes")]
    [CustomJwtAuthorize]
    [ProducesResponseType(typeof(XElement), StatusCodes.Status200OK)]
    public ActionResult GetJobCodes(string revision = null)
    {
        RequestTelemetry reqTelemetry = HttpContext?.Features?.Get<RequestTelemetry>();
        reqTelemetry?.Properties?.Add("SerialNumber", _terminalConfiguration.SerialNumber);

        try
        {
            var result = new XElement("jobCodes");

            var data = _jobCodeDataService.Query().ToList();

            var highestRevision = DateTimeOffset.MinValue;

            foreach (var jobCode in data)
            {
                result.Add(new XElement("jobCode",
                    new XElement("id", jobCode.Id),
                    new XElement("jobCategoryID", jobCode.JobCategoryId),
                    new XElement("code", jobCode.Code),
                    new XElement("name", jobCode.Name)
                    ));
                highestRevision = jobCode.LastModifiedOn;
            }

            if (highestRevision != DateTimeOffset.MinValue)
            {
                //Only put this in if there's a row
                result.AddFirst(new XElement("revision", highestRevision.ToString("O")));
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Something went wrong while calling [{nameof(GetJobCodes)}] for Device with SerialNumber '{_terminalConfiguration?.SerialNumber}'!");
            return Problem(detail: ex.Message);
        }
    }


}
