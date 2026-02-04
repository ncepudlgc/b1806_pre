using Bravia.Abstractions;

namespace Bravia;

internal class AVContentService : IAVContentService
{
    private const string Path = "avcontent/";
    
    private readonly IConnectionManager _connectionManager;
    private readonly IHttpRequestService _httpRequestService;
    private readonly IAVContentRequestFactory _avContentRequestFactory;
    
    public AVContentService(
        IConnectionManager connectionManager,
        IHttpRequestService httpRequestService,
        IBraviaRequestFactory braviaRequestFactory)
    {
        _connectionManager = connectionManager;
        _httpRequestService = httpRequestService;
        _avContentRequestFactory = braviaRequestFactory.AVContentRequestFactory;
    }
    
    public async Task<PlayContent> GetPlayingContentInfoAsync(string deviceId)
    {
        var connection = _connectionManager.GetConnection(deviceId);
        var request = _avContentRequestFactory.GetPlayingContentInfoRequest();

        var response = await _httpRequestService.SendAsync(Path, connection, request).ConfigureAwait(false);

        return await response.DeserializeAsync<PlayContent>().ConfigureAwait(false);
    }
    
    public async Task<bool> SetPlayContentAsync(string deviceId, SetPlayContentDto content)
    {
        var connection = _connectionManager.GetConnection(deviceId);
        var request = _avContentRequestFactory.SetPlayContentRequest(content);
        
        var response = await _httpRequestService.SendAsync(Path, connection, request).ConfigureAwait(false);

        await response.DeserializeAsync<PlayContent>().ConfigureAwait(false);

        return true;
    }
}