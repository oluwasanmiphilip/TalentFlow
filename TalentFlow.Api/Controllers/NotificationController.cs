using Microsoft.AspNetCore.Mvc;
using MediatR;
using TalentFlow.Application.Notifications.Commands;

//[ApiExplorerSettings(GroupName = "v5")]
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;
    public NotificationController(IMediator mediator) => _mediator = mediator;

    /// <summary>Send a notification</summary>
    [HttpPost]
    public async Task<ActionResult> SendNotification(SendNotificationCommand command)
    {
        var result = await _mediator.Send(command);
        return result ? Ok() : BadRequest("Notification failed");
    }

    /// <summary>Get notifications for a user</summary>
    [HttpGet("{userId}")]
    public async Task<ActionResult<List<NotificationDto>>> GetNotifications(Guid userId)
    {
        var notifications = await _mediator.Send(new GetNotificationsByUserQuery(userId));
        return Ok(notifications);
    }
}
