using MediatR;

namespace TalentFlow.Application.Notifications.Commands
{
    // Command to send a notification to a user
    public record SendNotificationCommand(Guid UserId, string Message) : IRequest<bool>;
}
