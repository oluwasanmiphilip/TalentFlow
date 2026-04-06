using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Enrollment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<List<Enrollment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
        Task UpdateAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    }
}
