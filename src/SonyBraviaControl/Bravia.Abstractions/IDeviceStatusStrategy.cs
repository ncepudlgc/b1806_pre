namespace Bravia.Abstractions;

public interface IDeviceStatusStrategy<TStatus> where TStatus : Status
{
    public Task<TStatus> CheckStatusAsync(string deviceId);
}