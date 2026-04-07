using Microsoft.EntityFrameworkCore;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly TalentFlowDbContext _context;

        public RoleRepository(TalentFlowDbContext context)
        {
            _context = context;
        }

        public Task<Role?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _context.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);

        public Task<Role?> GetByNameAsync(string name, CancellationToken ct) =>
            _context.Roles.FirstOrDefaultAsync(r => r.Name == name, ct);

        public async Task AddAsync(Role role, CancellationToken ct)
        {
            await _context.Roles.AddAsync(role, ct);
        }

        public Task UpdateAsync(Role role, CancellationToken ct)
        {
            _context.Roles.Update(role);
            return Task.CompletedTask;
        }

        // ✅ Match interface exactly
        public async Task<List<Role>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Roles.ToListAsync(ct);
        }
    }
}
