using System.Text.Json;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Bravia.DeviceHost;

public class HostService : IHttpApplication<HttpContext>
{
    private readonly SystemService _systemService;
    private readonly ConnectionService _connectionService;
    
    public HostService(
        SystemService systemService,
        ConnectionService connectionService)
    {
        _systemService = systemService;
        _connectionService = connectionService;
    }
    
    public HttpContext CreateContext(
        IFeatureCollection contextFeatures)
    {
        return new DefaultHttpContext(contextFeatures);
    }

    public async Task ProcessRequestAsync(HttpContext context)
    {
        var braviaRequest = await JsonSerializer.DeserializeAsync<BraviaHttpRequest>(
            context.Request.Body,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new Exception("Invalid request body.");
        
        var path = context.Request.Path.Value?.ToLowerInvariant();

        if(!TryGetIdFromPath(path, out var id))
        {
            throw new ArgumentException($"Invalid ID.");
        }

        string response = string.Empty;
        
        if ( path == "/system")
        {
            response = await _systemService.ProcessRequestAsync(id, braviaRequest).ConfigureAwait(false);
        }
        
        await context.Response.WriteAsync(response).ConfigureAwait(false);
    }

    public void DisposeContext(HttpContext context, Exception exception)
    {
    }
    
    private static bool TryGetIdFromPath(PathString path, out string id)
    {
        id = string.Empty;
        var parts = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts?.Length > 0 && Guid.TryParse(parts[0], out Guid guid))
        {
            id = guid.ToString();
            return true;
        }
        return false;
    }
}