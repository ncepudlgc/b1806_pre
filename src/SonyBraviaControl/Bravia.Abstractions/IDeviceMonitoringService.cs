namespace Bravia.Abstractions;

public interface IDeviceMonitoringService
{
    IObservable<PowerStatus> PowerStatusUpdates(string deviceId);
    IObservable<PlayContent> PlayContentUpdates(string deviceId);
}