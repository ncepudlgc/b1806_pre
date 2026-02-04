using Bravia.Abstractions;

namespace Bravia;

internal class SystemService : ISystemService
{
    private const string Path = "system/";
    
    private readonly IConnectionManager _connectionManager;
    private readonly IHttpRequestService _httpRequestService;
    private readonly ISystemRequestFactory _systemRequestFactory;
    
    public SystemService(
        IConnectionManager connectionManager,
        IHttpRequestService httpRequestService,
        IBraviaRequestFactory braviaRequestFactory)
    {
        _connectionManager = connectionManager;
        _httpRequestService = httpRequestService;
        _systemRequestFactory = braviaRequestFactory.SystemRequestFactory;
    }
    
    public async Task<PowerStatus> GetPowerStatusAsync(string deviceId)
    {
        var connection = _connectionManager.GetConnection(deviceId);
        var request = _systemRequestFactory.GetPowerStatusRequest();

        try
        {
            var response = await _httpRequestService.SendAsync(Path, connection, request).ConfigureAwait(false);

            return await response.DeserializeAsync<PowerStatus>().ConfigureAwait(false);   
        }
        catch (Exception e)
        {
            Console.WriteLine($" Error during {nameof(GetPowerStatusAsync)}: {e.Message}).");
            return new PowerStatus();
        }
    }

    public async Task<bool> SetPowerStatusAsync(string deviceId, SetPowerStatusDto status)
    {
        var connection = _connectionManager.GetConnection(deviceId);
        var request = _systemRequestFactory.SetPowerStatusRequest(status);

        try
        {
            var response = await _httpRequestService.SendAsync(Path, connection, request).ConfigureAwait(false);

            var result = await response.DeserializeAsync<PowerStatus>().ConfigureAwait(false);
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($" Error during {nameof(SetPowerStatusAsync)}: {e.Message}).");
            return false;
        }
    }
}