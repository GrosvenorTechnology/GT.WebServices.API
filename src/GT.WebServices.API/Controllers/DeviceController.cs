using GT.WebServices.API.Application.Dtos;
using GT.WebServices.API.Application.Security;
using GT.WebServices.API.Core;
using GT.WebServices.API.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace GT.WebServices.API.Controllers;

[ApiController]
[Route("api")]
public class DeviceController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly TelemetryClient _telemetryClient;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITerminalConfiguration _terminalConfiguration;
    private readonly EmployeeDataService _employeeDataService;
    private readonly IDataCollectionService _dataCollectionService;
    private readonly JobCategoryDataService _jobCategoryDataService;
    private readonly JobCodeDataService _jobCodeDataService;

    public DeviceController(ILogger<DeviceController> logger, IWebHostEnvironment webHostEnvironment, ITerminalConfiguration terminalConfiguration,
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

    [HttpPost("devices/{deviceID}")]
    public ActionResult<DeviceTokenDto> RegisterDevice(string deviceID, [FromBody] DeviceRegistrationDto deviceRegistration)
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
}
