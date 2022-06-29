using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using AutoMapper;

using GT.WebServices.API.Application.Attributes;
using GT.WebServices.API.Application.Dtos;
using GT.WebServices.API.Application.Security;
using GT.WebServices.API.Core;
using GT.WebServices.API.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Microsoft.ApplicationInsights.DataContracts;
using GT.WebServices.API.Domain;

namespace GT.WebServices.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly TelemetryClient _telemetryClient;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ITerminalConfiguration _terminalConfiguration;
        private readonly IEmployeeDataService _employeeDataService;
        private readonly IDataCollectionService _dataCollectionService;

        public DataController(ILogger<DataController> logger, IWebHostEnvironment webHostEnvironment, ITerminalConfiguration terminalConfiguration
              , IMapper mapper, TelemetryClient telemetryClient, IConfiguration configuration, IJwtTokenService jwtTokenService
              , IEmployeeDataService employeeDataService, IDataCollectionService dataCollectionService)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _telemetryClient = telemetryClient;
            _jwtTokenService = jwtTokenService;
            _terminalConfiguration = terminalConfiguration;
            _employeeDataService = employeeDataService;
            _dataCollectionService = dataCollectionService;
        }

        [HttpPost("devices/{deviceID}")]
        public async Task<ActionResult<DeviceTokenDto>> RegisterDevice(string deviceID, [FromBody] DeviceRegistrationDto deviceRegistration)
        {
            var serialNumber = _terminalConfiguration.SerialNumber;
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                _logger.LogWarning($"Device Registration failed because request doesn't contain a valid SerialNumber for serialNumber '{serialNumber}'!");
                return StatusCode(StatusCodes.Status403Forbidden, deviceRegistration);
            }

            // Check in your database to make shure device is valid

            //var device = await _dbContext.Devices.SingleOrDefaultAsync(x => x.DeviceId == deviceID);
            //if (device == null)
            //{
            //   _logger.LogWarning($"Device Registration failed because no existing Device could be found for deviceId '{deviceID}'!");
            //   return NotFound(deviceID);
            //}

            //if (device.Psk != deviceRegistration.Credentials.Password)
            //{
            //   _logger.LogWarning($"Device Registration failed because the credentials could not be validated for serialNumber '{serialNumber}'!");
            //   return Unauthorized();
            //}

            _logger.LogTrace($"Device with SerialNumber '{serialNumber}' registered successfully.");

            var token = _jwtTokenService.GenerateToken(serialNumber);
            return Ok(new DeviceTokenDto { Token = token });
        }

        [HttpGet("changes/{deviceID}")]
        [CustomJwtAuthorize]
        public async Task<ActionResult> GetChanges(
            string deviceID,
            string empsRevision = null, int? empsCount = null,
            string jobCodesRevision = null, int? jobCodesCount = null,
            string jobCategoriesRevision = null, int? jobCategoriesCount = null,
            string schedulesRevision = null, int? schedulesCount = null,
            string dataCollectionRevision = null
        )
        {
            async Task<bool> hasEmployeeChanges(string serialNumber)
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
                    empsAfterRevision = query.Any(x => x.ModifiedOn > revision.UtcDateTime);
                }

                return empsAfterRevision || empsCountsDontMatch;
            }

            var changes = new XElement("changes");

            try
            {
                var serialNumber = _terminalConfiguration.SerialNumber;

                if (await hasEmployeeChanges(serialNumber)) changes.Add(new XElement("employees"));
                
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
        public async Task<ActionResult> GetEmployees(string revision = null)
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
        public async Task<ActionResult> GetEmployeeIds()
        {
            var result = new XElement("employeeIDs");

            try
            {
                var serialNumber = _terminalConfiguration.SerialNumber;

                var employees = await _employeeDataService.GetCurrentEmployeeIds(serialNumber);
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


        [HttpPost("transaction/offline")]
        [CustomJwtAuthorize]
        public async Task<ActionResult> ProcessOfflineTransactions()
        {
            try
            {
                using StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
                var transactionMessage = await reader.ReadToEndAsync();

                // Store transaction somewhere now, make sore to throw an exception here if store fails

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Something went wrong while calling [{nameof(ProcessOfflineTransactions)}] for Device with SerialNumber '{_terminalConfiguration?.SerialNumber}'!\r\n{ex.Message}");

                //NOTE: Do not return 2XX here, this must be a 500 or the terminal will clear the message
                return Problem();
            }
        }

        [HttpPost("transaction/online")]
        [CustomJwtAuthorize]
        public async Task<ActionResult> ProcessOnlineTransactions()
        {
            var result = new XElement("response");
            var failedlMessage = "Clocking denied by server!";

            try
            {
                using StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
                var transactionMessage = await reader.ReadToEndAsync();

                // Store transaction somewhere now, make sore to throw an exception here if store fails

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Something went wrong while calling [{nameof(ProcessOnlineTransactions)}] for Device with SerialNumber '{_terminalConfiguration?.SerialNumber}'!\r\n{ex.Message}");
                result.Add(new XAttribute("failed", "true"), new XElement("message", failedlMessage));

                //NOTE: Do not return 2XX here, this must be a 500 or the terminal will clear the message
                return Problem();
            }
        }

    }
}
