using Application.Notifications.Commands.MarkAsRead;
using Application.Notifications.Queries.GetMyNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("GetMyNotifications", Name = "GetMyNotifications")]
    public async Task<IActionResult> GetMyNotifications([FromQuery] bool unreadOnly = false, CancellationToken ct = default)
    {
        var notifications = await _mediator.Send(new GetMyNotificationsQuery(unreadOnly), ct);
        return Ok(notifications);
    }

    [HttpPut("{id}/mark-as-read", Name = "MarkAsRead")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken ct)
    {
        try
        {
            await _mediator.Send(new MarkAsReadCommand(id), ct);
            return Ok(new { message = "Notification marked as read." });
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }
}
