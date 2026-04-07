using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IRoleRepository
    {
        Task AddAsync(Role role, CancellationToken cancellationToken = default);
        Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Role?> GetByNameAsync(string name, CancellationToken ct);
        Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
