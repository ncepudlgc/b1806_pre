using System.Collections.Concurrent;
using System.Threading.Channels;
using Bravia.Abstractions;

namespace Bravia;

/// <summary>
/// Service for managing device change notifications
/// </summary>
internal class NotificationService : INotificationService
{
    private readonly IDeviceMonitoringService _deviceMonitoringService;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _subscriptions = new();
    private readonly ConcurrentDictionary<string, Channel<DeviceChangeNotification>> _notificationChannels = new();

    public NotificationService(IDeviceMonitoringService deviceMonitoringService)
    {
        _deviceMonitoringService = deviceMonitoringService ?? throw new ArgumentNullException(nameof(deviceMonitoringService));
    }

    public async IAsyncEnumerable<DeviceChangeNotification> SubscribeAsync(
        string deviceId, 
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            throw new ArgumentException("Device ID cannot be null or empty", nameof(deviceId));
        }

        // Cancel existing subscription if any
        Unsubscribe(deviceId);

        // Create cancellation token source for this subscription
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _subscriptions.TryAdd(deviceId, cts);

        // Create channel for notifications
        var channel = Channel.CreateUnbounded<DeviceChangeNotification>();
        _notificationChannels.TryAdd(deviceId, channel);

        // Subscribe to power status updates
        var powerStatusSubscription = _deviceMonitoringService.PowerStatusUpdates(deviceId)
            .Subscribe(
                powerStatus =>
                {
                    var notification = new DeviceChangeNotification
                    {
                        DeviceId = deviceId,
                        ChangeType = "PowerStatus",
                        Data = powerStatus,
                        Timestamp = DateTime.UtcNow
                    };
                    channel.Writer.TryWrite(notification);
                },
                error =>
                {
                    channel.Writer.TryComplete(error);
                },
                () =>
                {
                    channel.Writer.TryComplete();
                });

        // Subscribe to play content updates
        var playContentSubscription = _deviceMonitoringService.PlayContentUpdates(deviceId)
            .Subscribe(
                playContent =>
                {
                    var notification = new DeviceChangeNotification
                    {
                        DeviceId = deviceId,
                        ChangeType = "PlayContent",
                        Data = playContent,
                        Timestamp = DateTime.UtcNow
                    };
                    channel.Writer.TryWrite(notification);
                },
                error =>
                {
                    channel.Writer.TryComplete(error);
                },
                () =>
                {
                    channel.Writer.TryComplete();
                });

        // Wait for cancellation and clean up
        try
        {
            await foreach (var notification in channel.Reader.ReadAllAsync(cts.Token))
            {
                yield return notification;
            }
        }
        finally
        {
            // Clean up subscriptions
            powerStatusSubscription?.Dispose();
            playContentSubscription?.Dispose();
            Unsubscribe(deviceId);
        }
    }

    public void Unsubscribe(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            return;
        }

        // Cancel subscription
        if (_subscriptions.TryRemove(deviceId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }

        // Complete channel
        if (_notificationChannels.TryRemove(deviceId, out var channel))
        {
            channel.Writer.TryComplete();
        }
    }
}
