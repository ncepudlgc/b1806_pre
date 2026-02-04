using System.Text.Json;
using Bravia.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Bravia.Api.Controllers;

/// <summary>
/// API controller for device change notifications.
/// Provides endpoints for subscribing and unsubscribing to device change notifications.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Subscribes to device change notifications using Server-Sent Events (SSE).
    /// </summary>
    /// <param name="id">Optional device ID. If not provided, monitors all devices.</param>
    /// <returns>Server-Sent Events stream of device change notifications.</returns>
    [HttpGet("{id?}/subscribe")]
    [Produces("text/event-stream")]
    public async Task SubscribeAsync(string? id = null, CancellationToken cancellationToken = default)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Connection", "keep-alive");

        try
        {
            await foreach (var notification in _notificationService.SubscribeAsync(id, cancellationToken))
            {
                var json = JsonSerializer.Serialize(notification);
                await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);

                _logger.LogDebug("Sent notification for device {DeviceId}, change type: {ChangeType}", 
                    notification.DeviceId, notification.ChangeType);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Subscription cancelled for device {DeviceId}", id ?? "all");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in notification subscription for device {DeviceId}", id ?? "all");
        }
    }

    /// <summary>
    /// Unsubscribes from device change notifications.
    /// Note: In a real implementation, you might want to track subscription IDs.
    /// For SSE, the connection closure automatically cancels the subscription.
    /// </summary>
    /// <param name="id">Optional device ID.</param>
    /// <param name="subscriptionId">The subscription ID to cancel.</param>
    [HttpPost("{id?}/unsubscribe")]
    public IActionResult Unsubscribe(string? id = null, [FromBody] string? subscriptionId = null)
    {
        if (string.IsNullOrEmpty(subscriptionId))
        {
            return BadRequest("subscriptionId is required");
        }

        _notificationService.Unsubscribe(subscriptionId);
        _logger.LogInformation("Unsubscribed from notifications for device {DeviceId}, subscription: {SubscriptionId}", 
            id ?? "all", subscriptionId);

        return Ok(new { message = "Unsubscribed successfully" });
    }
}
