using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Bravia.Abstractions;

namespace Bravia.Api.Controllers;

[Route("{id?}/notifications")]
[ApiController]
public class NotificationsController : BraviaController
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public override string ControllerName => "notifications";

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Subscribe to device change notifications using Server-Sent Events (SSE)
    /// </summary>
    /// <param name="id">Device ID to subscribe to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Server-Sent Events stream</returns>
    [HttpGet]
    [Route("subscribe")]
    public async Task SubscribeAsync(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Response.StatusCode = 400;
            await Response.WriteAsync("Device ID is required", cancellationToken);
            return;
        }

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

                _logger.LogInformation("Sent notification for device {DeviceId}: {ChangeType}", 
                    notification.DeviceId, notification.ChangeType);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Subscription cancelled for device {DeviceId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in notification stream for device {DeviceId}", id);
            await Response.WriteAsync($"data: {{\"error\": \"{ex.Message}\"}}\n\n", cancellationToken);
        }
        finally
        {
            _notificationService.Unsubscribe(id);
            _logger.LogInformation("Unsubscribed from device {DeviceId}", id);
        }
    }

    /// <summary>
    /// Unsubscribe from device change notifications
    /// </summary>
    /// <param name="id">Device ID to unsubscribe from</param>
    /// <returns>Success response</returns>
    [HttpPost]
    [Route("unsubscribe")]
    public IActionResult Unsubscribe([FromRoute] string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Device ID is required");
        }

        try
        {
            _notificationService.Unsubscribe(id);
            _logger.LogInformation("Unsubscribed from device {DeviceId}", id);
            return Ok(new { message = $"Unsubscribed from device {id}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unsubscribing from device {DeviceId}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
