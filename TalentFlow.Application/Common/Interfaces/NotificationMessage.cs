namespace TalentFlow.Application.Common.Interfaces
{
    public class NotificationMessage
    {
        public Guid LearnerId { get; set; }   // ✅ Guid instead of string
        public string DeepLinkUrl { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
