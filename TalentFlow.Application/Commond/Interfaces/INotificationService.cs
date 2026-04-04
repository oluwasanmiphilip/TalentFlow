namespace TalentFlow.Application.Common.Interfaces
{
    public interface INotificationService 
    {
        Task SendAsync(NotificationMessage notificationMessage);
        Task SendNotificationAsync(Guid notificationId, CancellationToken cancellationToken = default);
    }
}



public class NotificationMessage
{
    public string LearnerId { get; set; } = string.Empty;
    public string DeepLinkUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

