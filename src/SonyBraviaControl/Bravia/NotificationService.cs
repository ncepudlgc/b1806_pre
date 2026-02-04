using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading.Channels;
using Bravia.Abstractions;

namespace Bravia;

/// <summary>
/// Implementation of INotificationService that manages device change notifications.
/// Uses IDeviceMonitoringService to observe device changes and streams them to subscribers.
/// </summary>
internal class NotificationService : INotificationService
{
    private readonly IDeviceMonitoringService _deviceMonitoringService;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _subscriptions = new();
    private readonly ConcurrentDictionary<string, Channel<DeviceChangeNotification>> _channels = new();

    public NotificationService(IDeviceMonitoringService deviceMonitoringService)
    {
        _deviceMonitoringService = deviceMonitoringService ?? throw new ArgumentNullException(nameof(deviceMonitoringService));
    }

    public async IAsyncEnumerable<DeviceChangeNotification> SubscribeAsync(
        string? deviceId = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var subscriptionId = Guid.NewGuid().ToString();
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _subscriptions.TryAdd(subscriptionId, cts);

        var channel = Channel.CreateUnbounded<DeviceChangeNotification>();
        _channels.TryAdd(subscriptionId, channel);

        try
        {
            // Start monitoring device changes
            _ = Task.Run(async () =>
            {
                try
                {
                    await MonitorDeviceChangesAsync(deviceId, channel.Writer, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Expected when subscription is cancelled
                }
                finally
                {
                    channel.Writer.Complete();
                }
            }, cts.Token);

            // Stream notifications to the caller
            await foreach (var notification in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return notification;
            }
        }
        finally
        {
            Unsubscribe(subscriptionId);
        }
    }

    public void Unsubscribe(string subscriptionId)
    {
        if (_subscriptions.TryRemove(subscriptionId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }

        if (_channels.TryRemove(subscriptionId, out var channel))
        {
            channel.Writer.TryComplete();
        }
    }

    private async Task MonitorDeviceChangesAsync(
        string? deviceId,
        ChannelWriter<DeviceChangeNotification> writer,
        CancellationToken cancellationToken)
    {
        // If deviceId is specified, monitor only that device
        if (!string.IsNullOrEmpty(deviceId))
        {
            var powerObservable = _deviceMonitoringService.PowerStatusUpdates(deviceId)
                .Skip(1) // Skip initial value
                .DistinctUntilChanged()
                .Select(status => new DeviceChangeNotification
                {
                    DeviceId = deviceId,
                    ChangeType = "PowerStatus",
                    Data = status
                });

            var playContentObservable = _deviceMonitoringService.PlayContentUpdates(deviceId)
                .Skip(1) // Skip initial value
                .DistinctUntilChanged()
                .Select(content => new DeviceChangeNotification
                {
                    DeviceId = deviceId,
                    ChangeType = "PlayContent",
                    Data = content
                });

            await powerObservable
                .Merge(playContentObservable)
                .ForEachAsync(async notification =>
                {
                    await writer.WriteAsync(notification, cancellationToken);
                }, cancellationToken);
        }
        else
        {
            // Monitor all devices (this is a simplified implementation)
            // In a real scenario, you might need to track all known device IDs
            // For now, we'll monitor a default device or require deviceId to be provided
            throw new NotSupportedException("Monitoring all devices requires a deviceId to be specified.");
        }
    }
}
