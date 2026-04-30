using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Notifications.Commands;
using TalentFlow.Application.Notifications.Queries;
using TalentFlow.Application.Notifications.DTOs;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly INotificationRepository _repo;

        public NotificationController(IMediator mediator, INotificationRepository repo)
        {
            _mediator = mediator;
            _repo = repo;
        }

        // GET: api/notification
        [HttpGet]
        public async Task<ActionResult<List<NotificationDto>>> GetAllNotifications()
        {
            var notifications = await _mediator.Send(new GetAllNotificationsQuery());
            if (notifications == null || notifications.Count == 0) return NotFound();

            return Ok(notifications);
        }

        // GET: api/notification/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(Guid id)
        {
            var notification = await _repo.GetByIdAsync(id);
            if (notification == null) return NotFound();

            return Ok(notification);
        }

        // GET: api/notification/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<NotificationDto>>> GetNotificationsByUser(Guid userId)
        {
            var notifications = await _mediator.Send(new GetNotificationsByUserQuery(userId));
            if (notifications == null || notifications.Count == 0) return NotFound();

            return Ok(notifications);
        }

        // POST: api/notification
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] Notification notification)
        {
            await _repo.AddAsync(notification);
            return Ok(new { message = "Notification created", notification.Id });
        }

        // DELETE: api/notification/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var deletedBy = User.FindFirst("learner_id")?.Value ?? "system";
            var command = new DeleteNotificationCommand(id, deletedBy);

            var result = await _mediator.Send(command);
            return result ? Ok(new { message = "Notification deleted" }) : NotFound();
        }
    }
}
