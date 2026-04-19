using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface ILessonRepository
    {
        Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Lesson lesson, CancellationToken cancellationToken);
        Task UpdateAsync(Lesson lesson, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);

        // ✅ Add these
        Task<List<Lesson>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<(List<Lesson> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Lesson?> GetNextLessonAsync(Guid courseId, Guid currentLessonId, CancellationToken cancellationToken);
    }
}
