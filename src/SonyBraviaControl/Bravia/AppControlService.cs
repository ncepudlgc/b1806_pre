using Bravia.Abstractions;

namespace Bravia;

internal class AppControlService : IAppControlService
{
    private const string Path = "system/";
    
    private readonly IConnectionManager _connectionManager;
    private readonly IHttpRequestService _httpRequestService;
    
    public AppControlService(
        IConnectionManager connectionManager,
        IHttpRequestService httpRequestService)
    {
        _connectionManager = connectionManager;
        _httpRequestService = httpRequestService;
    }
}