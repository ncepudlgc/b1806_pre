// SonyBraviaControl/Bravia.DeviceHost/Startup.cs
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bravia.DeviceHost;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<SystemService>();
        services.AddSingleton<ConnectionService>();
        services.AddSingleton<AVContentService>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            var braviaRequest = await JsonSerializer.DeserializeAsync<BraviaHttpRequest>(
                context.Request.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new Exception("Invalid request body.");
        
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? throw new Exception("Invalid path.");
            var hostname = context.Request.Host.Value?.ToLowerInvariant() ?? throw new Exception("Invalid hostname.");
            string response;
        
            if (path.Equals("/sony/system/", StringComparison.InvariantCultureIgnoreCase))
            {
                var systemService = context.RequestServices.GetRequiredService<SystemService>();
                response = await systemService.ProcessRequestAsync(hostname, braviaRequest).ConfigureAwait(false);
            }
            else if (path.Equals("/sony/avcontent/", StringComparison.InvariantCultureIgnoreCase))
            {
                var avContentService = context.RequestServices.GetRequiredService<AVContentService>();
                response = await avContentService.ProcessRequestAsync(hostname, braviaRequest).ConfigureAwait(false);
            }
            else
            {
                context.Response.StatusCode = 404;
                response = "Not Found";
            }
            await context.Response.WriteAsync(response).ConfigureAwait(false);
        });
    }
}