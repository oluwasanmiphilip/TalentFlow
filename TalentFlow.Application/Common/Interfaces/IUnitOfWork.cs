using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IRoleRepository Roles { get; }
        IAuditLogRepository AuditLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public interface IEventStreamPublisher
    {
        Task PublishAsync(string eventName, object payload, CancellationToken cancellationToken = default);
    }

    public interface IJwtTokenService
    {
        string GenerateToken(string learnerId, string email);
    }

    public interface INotificationService
    {
        Task SendAsync(NotificationMessage notificationMessage);
        Task SendNotificationAsync(Guid notificationId, CancellationToken cancellationToken = default);
    }
}
