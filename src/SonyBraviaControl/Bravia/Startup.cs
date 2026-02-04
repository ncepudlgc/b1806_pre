using Bravia.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bravia;

public static class Startup
{
    public static IServiceCollection AddBraviaServices(this IServiceCollection services)
    {
        services.AddSingleton<IAudioService, AudioService>();
        services.AddSingleton<ISystemService, SystemService>();
        services.AddSingleton<IAVContentService, AVContentService>();
        services.AddSingleton<IAppControlService, AppControlService>();
        
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddSingleton<IHttpRequestService, HttpRequestService>();
        
        services.AddSingleton<IBraviaRequestFactory, BraviaRequestFactory>();
        services.AddSingleton<ISystemRequestFactory, SystemRequestFactory>();
        services.AddSingleton<IAVContentRequestFactory, AVContentRequestFactory>();
        
        services.AddSingleton<IDeviceMonitoringService, DeviceMonitoringService>();
        services.AddSingleton<IDeviceStatusStrategy<PowerStatus>, PowerStatusStrategy>();
        services.AddSingleton<IDeviceStatusStrategy<PlayContent>, PlayContentStrategy>();
        
        return services;
    }
}