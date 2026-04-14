using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetNotificationsByUserIdAsync(Guid userId);
        Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<Notification>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(Notification notification, CancellationToken ct = default);
        Task UpdateAsync(Notification notification, CancellationToken ct = default);
        Task SoftDeleteAsync(Notification notification, CancellationToken ct = default);
    }
}
