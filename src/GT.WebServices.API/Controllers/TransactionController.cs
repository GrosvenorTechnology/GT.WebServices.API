using System.Text;
using System.Xml.Linq;

using GT.WebServices.API.Application.Attributes;
using GT.WebServices.API.Core;
using Microsoft.AspNetCore.Mvc;

namespace GT.WebServices.API.Controllers;

[ApiController]
[Route("api")]
public class TransactionController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly ITerminalConfiguration _terminalConfiguration;

    public TransactionController(ILogger<TransactionController> logger, ITerminalConfiguration terminalConfiguration)
    {
        _logger = logger;
        _terminalConfiguration = terminalConfiguration;
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
