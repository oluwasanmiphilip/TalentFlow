using MediatR;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Notifications.Commands
{
    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, bool>
    {
        private readonly INotificationService _notificationService;

        public SendNotificationCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            await _notificationService.SendAsync(new NotificationMessage
            {
                LearnerId = request.UserId,   // ✅ Guid directly
                DeepLinkUrl = "/me/profile",
                Message = request.Message
            });

            return true;
        }
    }
}
