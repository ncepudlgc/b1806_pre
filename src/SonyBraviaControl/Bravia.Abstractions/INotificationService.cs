namespace Bravia.Abstractions;

/// <summary>
/// Service for managing device change notifications.
/// Allows clients to subscribe and receive notifications when device monitor detects changes.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Subscribes to device change notifications for a specific device.
    /// Returns an async enumerable that streams notifications.
    /// </summary>
    /// <param name="deviceId">The device ID to monitor. If null or empty, monitors all devices.</param>
    /// <param name="cancellationToken">Cancellation token to stop the subscription.</param>
    /// <returns>Async enumerable of device change notifications.</returns>
    IAsyncEnumerable<DeviceChangeNotification> SubscribeAsync(string? deviceId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Unsubscribes from device change notifications for a specific subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID to cancel.</param>
    void Unsubscribe(string subscriptionId);
}

/// <summary>
/// Represents a notification when a device change is detected.
/// </summary>
public class DeviceChangeNotification
{
    public string DeviceId { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty; // "PowerStatus" or "PlayContent"
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public object? Data { get; set; }
}
