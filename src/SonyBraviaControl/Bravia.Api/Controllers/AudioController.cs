using Microsoft.AspNetCore.Mvc;
using Bravia.Abstractions;

namespace Bravia.Api.Controllers;

[Route("{id?}/[controller]")]
public class AudioController : BraviaController
{
    private readonly IAudioService _audioService;
    private readonly ILogger<AudioController> _logger;
    private readonly IConnectionManager _connectionManager;

    public override string ControllerName => "audio";
    
    public AudioController(ILogger<AudioController> logger,
        IAudioService audioService,
        IConnectionManager connectionManager)
    {
        _logger = logger;
        _audioService = audioService;
        _connectionManager = connectionManager;
    }

    [HttpPut]
    public async Task<IActionResult> SetMuteAsync(
        [FromRoute] string id,
        [FromBody] bool status)
    {
        await _audioService.SetAudioMuteAsync(id, status).ConfigureAwait(false);

        return Ok();
    }
}