using Checkin.Common;
using Microsoft.AspNetCore.Mvc;
using Reservation.Checkin.API.Services;

namespace Reservation.Checkin.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CheckinController : ControllerBase
{
    private readonly ILogger<CheckinController> _logger;
    private readonly CheckinPublisher _checkinPublisher;
    private readonly CheckinService _checkinService;

    public CheckinController(
        ILogger<CheckinController> logger,
        CheckinPublisher checkinPublisher,
        CheckinService checkinService)
    {
        _logger = logger;
        _checkinPublisher = checkinPublisher;
        _checkinService = checkinService;
    }

    [HttpPost("accept")]
    public async Task<IActionResult> PostCheckinAccept(CheckinCommand command)
    {
        var id = await _checkinPublisher.PublishAsync(command);
        _logger.LogInformation($"command valid and published = {id}");

        return Accepted($"/checkin/{id}");
    }

    [HttpPost("redirect")]
    public async Task<IActionResult> PostCheckinRedirect(CheckinCommand command)
    {
        var id = await _checkinPublisher.PublishAsync(command);
        _logger.LogInformation($"command valid and published = {id}");

        return Redirect($"/checkin/{id}");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var response = await _checkinService.Fetch(id);
        if(response != null)
            return Ok(response);

        return NotFound();
    }
}

