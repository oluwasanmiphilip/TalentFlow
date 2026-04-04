using MediatR;
using TalentFlow.Application.Notifications.Commands;

public record GetNotificationsByUserQuery(Guid UserId) : IRequest<List<NotificationDto>>;
