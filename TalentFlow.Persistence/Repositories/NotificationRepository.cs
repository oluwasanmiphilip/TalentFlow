using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly TalentFlowDbContext _context;

        public NotificationRepository(TalentFlowDbContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted, ct);
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task<List<Notification>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Notifications
                .Where(n => !n.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Notification notification, CancellationToken ct = default)
        {
            await _context.Notifications.AddAsync(notification, ct);
        }

        public Task UpdateAsync(Notification notification, CancellationToken ct = default)
        {
            _context.Notifications.Update(notification);
            return Task.CompletedTask;
        }

        public Task SoftDeleteAsync(Notification notification, CancellationToken ct = default)
        {
            notification.SoftDelete(notification.DeletedBy ?? "system");
            _context.Notifications.Update(notification);
            return Task.CompletedTask;
        }
    }
}
