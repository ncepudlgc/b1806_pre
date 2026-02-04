// SonyBraviaControl/Bravia.Api/InputController.cs
using Microsoft.AspNetCore.Mvc;
using Bravia.Abstractions;


namespace Bravia.Api.Controllers;


[Route("{id?}/[controller]")]
public class InputController : BraviaController
{
    private readonly ILogger<InputController> _logger;
    private readonly IAVContentService _avContentService;
    
    public override string ControllerName => "input";
    
    public InputController(
        ILogger<InputController> logger,
        IAVContentService avContentService)
    {
        _logger = logger;
        _avContentService = avContentService;
    }


    [HttpGet]
    public IActionResult GetInputs(string deviceId)
    {
        return Ok();
    }
    
    [HttpPost("content/set")]
    public async Task<IActionResult> SetPlayContent(
        [FromRoute] string id,
        [FromBody] SetPlayContentDto content)
    {
        var result = await _avContentService.SetPlayContentAsync(id, content);
        return Ok(result);
    }
}