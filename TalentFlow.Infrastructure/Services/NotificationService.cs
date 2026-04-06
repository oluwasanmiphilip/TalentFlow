using System;
using System.Threading;
using System.Threading.Tasks;
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
            // Convert LearnerId (string) into Guid for UserId
            var userId = Guid.Parse(notificationMessage.LearnerId);

            // Use the entity constructor
            var notification = new Notification(userId, notificationMessage.Message);

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        // Mark an existing notification as sent
        public async Task SendNotificationAsync(Guid notificationId, CancellationToken cancellationToken = default)
        {
            var notification = await _context.Notifications.FindAsync(new object[] { notificationId }, cancellationToken);
            if (notification == null) return;

            // Use the domain method instead of setting SentAt directly
            notification.MarkAsSent();

            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
