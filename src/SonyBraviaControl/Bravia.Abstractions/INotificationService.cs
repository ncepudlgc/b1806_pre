using Bravia.Abstractions;

namespace Bravia.Abstractions;

/// <summary>
/// Service for managing device change notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Subscribes to device change notifications for a specific device
    /// </summary>
    /// <param name="deviceId">The device ID to subscribe to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async enumerable of device change notifications</returns>
    IAsyncEnumerable<DeviceChangeNotification> SubscribeAsync(string deviceId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Unsubscribes from device change notifications for a specific device
    /// </summary>
    /// <param name="deviceId">The device ID to unsubscribe from</param>
    void Unsubscribe(string deviceId);
}

/// <summary>
/// Represents a device change notification
/// </summary>
public class DeviceChangeNotification
{
    public string DeviceId { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty; // "PowerStatus" or "PlayContent"
    public object? Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
