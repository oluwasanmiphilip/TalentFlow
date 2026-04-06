namespace TalentFlow.Application.Common.Interfaces
{
    public class NotificationMessage
    {
        public string LearnerId { get; set; } = string.Empty;
        public string DeepLinkUrl { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
