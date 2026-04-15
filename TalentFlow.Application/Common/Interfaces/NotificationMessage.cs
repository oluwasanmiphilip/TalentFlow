namespace TalentFlow.Application.Common.Interfaces
{
    public class NotificationMessage
    {
        public Guid UserId { get; set; }
        public string Channel { get; set; } = "email"; // "email" or "sms"
        public string Message { get; set; } = string.Empty;

        public string RecipientEmail { get; set; } = string.Empty;
        public string RecipientPhoneNumber { get; set; } = string.Empty;

        // Add this if you want to include a deep link in the notification
        public string DeepLinkUrl { get; set; } = string.Empty;
    }
}
