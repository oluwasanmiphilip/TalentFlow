using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Notifications.Queries;
using TalentFlow.Application.Notifications.DTOs;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Notifications.Handlers
{
    public class GetAllNotificationsHandler
        : IRequestHandler<GetAllNotificationsQuery, List<NotificationDto>>
    {
        private readonly INotificationRepository _repo;

        public GetAllNotificationsHandler(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<NotificationDto>> Handle(GetAllNotificationsQuery request, CancellationToken ct)
        {
            var notifications = await _repo.GetAllAsync(ct);
            if (notifications == null || !notifications.Any()) return new List<NotificationDto>();

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id.ToString(),
                UserId = n.UserId,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsDeleted = n.IsDeleted,
                DeletedBy = n.DeletedBy,
                DeletedAt = n.DeletedAt
            }).ToList();
        }
    }
}
