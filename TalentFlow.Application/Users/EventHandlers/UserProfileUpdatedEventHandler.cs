using MediatR;
using TalentFlow.Application.Users.Events;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Users.EventHandlers
{
    public class UserProfileUpdatedEventHandler : INotificationHandler<UserProfileUpdatedNotification>
    {
        private readonly INotificationService _notificationService;
        private readonly IEventStreamPublisher _eventStream;

        public UserProfileUpdatedEventHandler(INotificationService notificationService, IEventStreamPublisher eventStream)
        {
            _notificationService = notificationService;
            _eventStream = eventStream;
        }

        public async Task Handle(UserProfileUpdatedNotification notification, CancellationToken cancellationToken)
        {
            var user = notification.DomainEvent.User;

            // Send profile update notification
            await _notificationService.SendAsync(new NotificationMessage
            {
                LearnerId = user.LearnerId,
                DeepLinkUrl = "/me/profile",
                Message = "Your profile has been updated successfully."
            });

            // Publish to event stream (Kafka, etc.)
            await _eventStream.PublishAsync("UserProfileUpdated", new
            {
                learner_id = user.LearnerId,
                name = user.FullName,
                updated_at = DateTime.UtcNow
            }, cancellationToken);
        }
    }
}
