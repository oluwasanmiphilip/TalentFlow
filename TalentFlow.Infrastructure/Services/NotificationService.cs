using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;
using TalentFlow.Persistence;

namespace TalentFlow.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly TalentFlowDbContext _context;

        public NotificationService(TalentFlowDbContext context)
        {
            _context = context;
        }

        // Send a new notification
        public async Task SendAsync(NotificationMessage notificationMessage)
        {
            var notification = new Notification(notificationMessage.LearnerId, notificationMessage.Message);
            notification.MarkAsSent(); // ✅ sets SentAt internally

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

        }


        // Mark an existing notification as sent
        public async Task SendNotificationAsync(Guid notificationId, CancellationToken cancellationToken = default)
        {
            var notification = await _context.Notifications.FindAsync(new object[] { notificationId }, cancellationToken);
            if (notification == null) return;

            notification.MarkAsSent();

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
