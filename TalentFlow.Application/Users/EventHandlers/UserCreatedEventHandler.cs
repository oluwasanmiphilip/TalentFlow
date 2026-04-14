using MediatR;
using TalentFlow.Application.Users.Events;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Users.EventHandlers
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedNotification>
    {
        private readonly INotificationService _notificationService;
        private readonly IEventStreamPublisher _eventStream;
        private readonly IUserRepository _userRepository; // ✅ add repository

        public UserCreatedEventHandler(
            INotificationService notificationService,
            IEventStreamPublisher eventStream,
            IUserRepository userRepository) // ✅ inject repository
        {
            _notificationService = notificationService;
            _eventStream = eventStream;
            _userRepository = userRepository;
        }

        public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            var userId = notification.DomainEvent.UserId;
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken); // ✅ now valid

            await _notificationService.SendAsync(new NotificationMessage
            {
                UserId = user.Id,
                DeepLinkUrl = "/me/profile",
                Message = $"Welcome {user.FullName}! Your account has been created."
            });

            await _eventStream.PublishAsync("UserCreated", new
            {
                user_id = user.Id.ToString(),
                email = user.Email,
                name = user.FullName,
                created_at = DateTime.UtcNow
            }, cancellationToken);
        }
    }
}
