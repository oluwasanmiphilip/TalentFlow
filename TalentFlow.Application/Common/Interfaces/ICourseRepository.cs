using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<Course>> GetByLearnerIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(Course course, CancellationToken cancellationToken = default);
        Task UpdateAsync(Course course, CancellationToken cancellationToken = default);
    }
}
