using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TalentFlow.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly TalentFlowDbContext _context;

        public RoleRepository(TalentFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        {
            await _context.Roles.AddAsync(role, cancellationToken);
        }

        public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Roles.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles.ToListAsync(cancellationToken);
        }
    }
}
