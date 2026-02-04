using System.Collections.Concurrent;
using System.Reactive.Subjects;
using Bravia.Abstractions;

namespace Bravia;

internal class DeviceMonitoringService : IDeviceMonitoringService
{
    /// <summary>
    ///     Sets the elapsed timer interval used to get status updates at a regular interval
    /// </summary>
    private const int IntervalMs = 5000;

    private static readonly PeriodicTimer Timer = new(TimeSpan.FromMilliseconds(IntervalMs));
    private static readonly ConcurrentDictionary<string, BehaviorSubject<PowerStatus>> DevicePowerStatuses = new();
    private static readonly ConcurrentDictionary<string, BehaviorSubject<PlayContent>> DevicePlayContentStatuses = new();

    private readonly IConnectionManager _connectionManager;
    private readonly IDeviceStatusStrategy<PowerStatus> _powerStatusStrategy;
    private readonly IDeviceStatusStrategy<PlayContent> _playContentStatusStrategy;

    public DeviceMonitoringService(
        IConnectionManager connectionManager,
        IDeviceStatusStrategy<PowerStatus> powerStatusStrategy,
        IDeviceStatusStrategy<PlayContent> playContentStatusStrategy)
    {
        _connectionManager = connectionManager;
        _powerStatusStrategy = powerStatusStrategy;
        _playContentStatusStrategy = playContentStatusStrategy;

        PollDevicesAsync().ConfigureAwait(false);
    }
    
    public IObservable<PowerStatus> PowerStatusUpdates(string deviceId)
    {
        if (DevicePowerStatuses.TryGetValue(deviceId, out var devicePowerStatus))
        {
            return devicePowerStatus;
        }

        BehaviorSubject<PowerStatus> newSubject = new(new PowerStatus());
        
        DevicePowerStatuses.TryAdd(deviceId, newSubject);
        
        return newSubject;
    }
    
    public IObservable<PlayContent> PlayContentUpdates(string deviceId)
    {
        if (DevicePlayContentStatuses.TryGetValue(deviceId, out var devicePlayContentStatus))
        {
            return devicePlayContentStatus;
        }

        BehaviorSubject<PlayContent> newSubject = new(new PlayContent());
        
        DevicePlayContentStatuses.TryAdd(deviceId, newSubject);
        
        return newSubject;
    }

    private async Task PollDevicesAsync()
    {
        List<Task> UpdateTasks = [];
        
        while (await Timer.WaitForNextTickAsync())
        {
            try
            {
                Console.WriteLine("polling devices");
                var activeIds = _connectionManager.GetConnectionIds().ToList();

                // Remove disconnected devices first.
                var removedConnectionsInPowerStatus = DevicePowerStatuses.Keys.Where(id => !activeIds.Contains(id));
                var removedConnectionsInPlayContent =
                    DevicePlayContentStatuses.Keys.Where(id => !activeIds.Contains(id));

                foreach (var removedDeviceId in removedConnectionsInPowerStatus)
                {
                    DevicePowerStatuses[removedDeviceId].OnCompleted();
                    DevicePowerStatuses.Remove(removedDeviceId, out _);
                }

                foreach (var removedDeviceId in removedConnectionsInPlayContent)
                {
                    DevicePlayContentStatuses[removedDeviceId].OnCompleted();
                    DevicePlayContentStatuses.Remove(removedDeviceId, out _);
                }

                foreach (var deviceId in DevicePowerStatuses.Keys)
                {
                    Task task = Task.Run(async () =>
                    {
                        var powerStatus = await _powerStatusStrategy.CheckStatusAsync(deviceId).ConfigureAwait(false);

                        if (powerStatus.Status.Equals(DevicePowerStatuses[deviceId].Value.Status))
                        {
                            return;
                        }
                        
                        var status = new PowerStatus
                        {
                            Status = powerStatus.Status
                        };
                        
                        DevicePowerStatuses[deviceId].OnNext(status);
                    });

                    UpdateTasks.Add(task);
                }

                foreach (var deviceId in DevicePlayContentStatuses.Keys)
                {
                    Task task = Task.Run(async () =>
                    {
                        var playContent = await _playContentStatusStrategy.CheckStatusAsync(deviceId).ConfigureAwait(false);

                        if (playContent.Uri.Equals(DevicePlayContentStatuses[deviceId].Value.Uri))
                        {
                            return;
                        }
                        
                        var content = new PlayContent
                        {
                            Uri = playContent.Uri,
                            Title = playContent.Title,
                            Source = playContent.Source,
                        };
                        
                        DevicePlayContentStatuses[deviceId].OnNext(content);
                    });
                
                    UpdateTasks.Add(task);
                }

                await Parallel.ForEachAsync(UpdateTasks,
                    async (task, cancellationToken) =>
                    {
                        await task.ConfigureAwait(false);
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error, {e}");
            }
        }
    }
}