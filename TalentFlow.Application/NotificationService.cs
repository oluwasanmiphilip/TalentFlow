using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Notifications.Events;

namespace TalentFlow.Application.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IMediator _mediator;

        public NotificationService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendAsync(NotificationMessage notificationMessage)
        {
            var evt = new NotificationSentEvent(Guid.NewGuid())
            {
                Message = notificationMessage.Message,
                LearnerId = notificationMessage.LearnerId,   // ✅ Guid directly
                DeepLinkUrl = notificationMessage.DeepLinkUrl
            };

            await _mediator.Publish(evt);
        }

        public async Task SendNotificationAsync(Guid notificationId, CancellationToken cancellationToken = default)
        {
            var evt = new NotificationSentEvent(notificationId);
            await _mediator.Publish(evt, cancellationToken);
        }
    }
}
