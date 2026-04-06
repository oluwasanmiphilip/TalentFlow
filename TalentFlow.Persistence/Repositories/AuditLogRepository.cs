using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TalentFlow.Persistence.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly TalentFlowDbContext _context;

        public AuditLogRepository(TalentFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
        {
            await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        }

        public async Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.AuditLogs.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<List<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.AuditLogs.ToListAsync(cancellationToken);
        }
    }
}
