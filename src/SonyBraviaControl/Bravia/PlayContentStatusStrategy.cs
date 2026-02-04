using Bravia.Abstractions;

namespace Bravia;

internal class PlayContentStrategy : IDeviceStatusStrategy<PlayContent>
{
    private readonly IAVContentService _avContentService;

    public PlayContentStrategy(IAVContentService avContentService)
    {
        _avContentService = avContentService;
    }

    public async Task<PlayContent> CheckStatusAsync(string deviceId)
    {
        return await _avContentService.GetPlayingContentInfoAsync(deviceId);
    }
}