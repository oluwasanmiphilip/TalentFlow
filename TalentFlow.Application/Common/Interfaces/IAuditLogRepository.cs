using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
        Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
