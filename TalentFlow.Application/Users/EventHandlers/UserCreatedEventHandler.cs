using MediatR;
using TalentFlow.Application.Users.Events;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Users.EventHandlers
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedNotification>
    {
        private readonly INotificationService _notificationService;
        private readonly IEventStreamPublisher _eventStream;

        public UserCreatedEventHandler(INotificationService notificationService, IEventStreamPublisher eventStream)
        {
            _notificationService = notificationService;
            _eventStream = eventStream;
        }

        public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            var user = notification.DomainEvent.User;

            // Send welcome notification
            await _notificationService.SendAsync(new NotificationMessage
            {
                LearnerId = user.LearnerId,
                DeepLinkUrl = "/me/profile",
                Message = $"Welcome {user.FullName}! Your account has been created."
            });

            // Publish to event stream (Kafka, etc.)
            await _eventStream.PublishAsync("UserCreated", new
            {
                learner_id = user.LearnerId,
                email = user.Email,
                name = user.FullName,
                created_at = DateTime.UtcNow
            }, cancellationToken);
        }
    }
}
