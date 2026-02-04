using Microsoft.AspNetCore.Mvc;
using Bravia.Abstractions;

namespace Bravia.Api.Controllers;

[Route("{id?}/[controller]")]
public class SettingsController : BraviaController
{
    private readonly ILogger<SettingsController> _logger;
    private readonly IConnectionManager _connectionManager;
    
    public override string ControllerName => "settings";
    
    public SettingsController(ILogger<SettingsController> logger,
        IConnectionManager connectionManager)
    {
        _logger = logger;
        _connectionManager = connectionManager;
    }
}