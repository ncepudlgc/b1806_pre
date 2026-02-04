namespace Bravia.Abstractions;

public interface IAudioService
{
    Task<bool> SetAudioMuteAsync(string deviceId, bool status);
    Task<bool> SetAudioVolumeAsync(string deviceId, int level);
}