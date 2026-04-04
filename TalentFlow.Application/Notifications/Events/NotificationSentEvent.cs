using MediatR;

namespace TalentFlow.Application.Notifications.Events
{
    public class NotificationSentEvent : INotification
    {
        public Guid NotificationId { get; }
        public string LearnerId { get; set; } = string.Empty;
        public string DeepLinkUrl { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public NotificationSentEvent(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
