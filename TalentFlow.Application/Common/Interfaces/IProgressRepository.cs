using System;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IProgressRepository
    {
        Task<LessonProgress?> GetByLearnerAndLessonAsync(Guid learnerId, Guid courseId, Guid lessonId, CancellationToken cancellationToken);

        Task AddAsync(LessonProgress progress, CancellationToken cancellationToken);

        Task UpdateAsync(LessonProgress progress, CancellationToken cancellationToken);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
