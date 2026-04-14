using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetCoursesByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<Course?> GetBySlugAsync(string slug, CancellationToken ct = default);
        Task<List<Course>> GetAllAsync(CancellationToken ct = default);
        Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Course course, CancellationToken ct = default);
        Task UpdateAsync(Course course, CancellationToken ct = default);
        Task SoftDeleteAsync(Course course, CancellationToken ct = default);

        // ✅ FIXED: return a list, not a single course
        Task<List<Course>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct = default);
    }
}
