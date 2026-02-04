using Microsoft.AspNetCore.Mvc;
using Bravia.Abstractions;

namespace Bravia.Api.Controllers;

[Route("{id?}/system")]
public class SystemController : BraviaController
{
    private const string Power = "power";
    
    private readonly ISystemService _systemService;
    private readonly ILogger<SystemController> _logger;
    
    public override string ControllerName => "system";
    
    public SystemController(ILogger<SystemController> logger,
        ISystemService systemService)
    {
        _logger = logger;
        _systemService = systemService;
    }

    [HttpGet]
    [Route(Power)]
    public async Task<IActionResult> GetStatusAsync(
        [FromRoute] string id)
    {
        var powerStatus = await _systemService.GetPowerStatusAsync(id).ConfigureAwait(false);

        return Ok(powerStatus);
    }

    [HttpPost]
    [Route(Power)]
    public async Task<IActionResult> SetPowerStatusAsync(
        [FromRoute] string id,
        [FromBody] SetPowerStatusDto status)
    {
        await _systemService.SetPowerStatusAsync(id, status).ConfigureAwait(false);

        return Ok();
    }
}