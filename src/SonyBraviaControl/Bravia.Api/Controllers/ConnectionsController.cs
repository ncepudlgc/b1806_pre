using Microsoft.AspNetCore.Mvc;
// using Bravia.Domain;
using Bravia.Abstractions;

namespace Bravia.Api.Controllers;

[Route("[controller]")]
public class ConnectionsController : BraviaController
{
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<ConnectionsController> _logger;

    public override string ControllerName => "connections";
    
    public ConnectionsController(ILogger<ConnectionsController> logger,
        IConnectionManager connectionManager)
    {
        _logger = logger;
        _connectionManager = connectionManager;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_connectionManager.GetConnections());
    }

    [HttpGet]
    [Route("{id?}")]
    public IActionResult Get(
        [FromRoute] string id)
    {
        return Ok(_connectionManager.GetConnection(id));
    }
    
    [HttpDelete]
    [Route("remove")]
    public IActionResult Remove([FromBody] string id)
    {
        return Ok(_connectionManager.RemoveConnection(id));
    }

    [HttpPost]
    [Route("add")]
    public IActionResult Add([FromBody] Connection connection)
    {
        var result = _connectionManager.AddConnection(connection.Id, connection);

        return Ok(result);
    }
}