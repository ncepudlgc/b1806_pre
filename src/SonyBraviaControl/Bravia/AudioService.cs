using Bravia.Abstractions;

namespace Bravia;

internal class AudioService : IAudioService
{
    public Task<bool> SetAudioMuteAsync(string deviceId, bool status)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetAudioVolumeAsync(string deviceId, int level)
    {
        throw new NotImplementedException();
    }
}