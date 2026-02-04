using Bravia.Abstractions;

namespace Bravia.Api;

public static class Startup
{
    public static IServiceCollection AddBraviaApi(this IServiceCollection services)
    {
        services.AddBraviaServices();
        
        return services;
    }
}