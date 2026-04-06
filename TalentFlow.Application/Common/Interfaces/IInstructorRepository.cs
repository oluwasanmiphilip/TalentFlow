using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IInstructorRepository
    {
        Task<Instructor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Instructor>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Instructor instructor, CancellationToken cancellationToken = default);
        Task UpdateAsync(Instructor instructor, CancellationToken cancellationToken = default);
    }
}
