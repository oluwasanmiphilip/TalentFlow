using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IAssessmentRepository
    {
        Task<Assessment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Assessment>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Assessment assessment, CancellationToken cancellationToken = default);
        Task UpdateAsync(Assessment assessment, CancellationToken cancellationToken = default);
    }
}
