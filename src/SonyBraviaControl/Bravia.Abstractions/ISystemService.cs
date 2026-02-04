namespace Bravia.Abstractions;

public interface ISystemService
{
    Task<PowerStatus> GetPowerStatusAsync(string deviceId);
    Task<bool> SetPowerStatusAsync(string deviceId, SetPowerStatusDto status);
}