// Application/Common/Interfaces/IEnrollmentRepository.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default);
        Task AddAsync(Enrollment enrollment, CancellationToken ct = default);
        Task UpdateAsync(Enrollment enrollment, CancellationToken ct = default);
        Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId, CancellationToken ct);
        Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken ct = default);

    }
}
