namespace Bravia.Abstractions;

public interface IAVContentService
{
    Task<PlayContent> GetPlayingContentInfoAsync(string deviceId);
    Task<bool> SetPlayContentAsync(string deviceId, SetPlayContentDto content);
}