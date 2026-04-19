using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly TalentFlowDbContext _context;

        public EnrollmentRepository(TalentFlowDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Enrollments.FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<List<Enrollment>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default)
        {
            return await _context.Enrollments.Where(e => e.CourseId == courseId && !e.IsDeleted).ToListAsync(ct);
        }

        public async Task<List<Enrollment>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Enrollments.Where(e => e.UserId == userId && !e.IsDeleted).ToListAsync(ct);
        }

        public async Task AddAsync(Enrollment enrollment, CancellationToken ct = default)
        {
            await _context.Enrollments.AddAsync(enrollment, ct);
        }

        public Task UpdateAsync(Enrollment enrollment, CancellationToken ct = default)
        {
            _context.Enrollments.Update(enrollment);
            return Task.CompletedTask;
        }

        public async Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId, CancellationToken ct)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId, ct);
        }
        public Task SoftDeleteAsync(Enrollment enrollment, CancellationToken ct = default)
        {
            enrollment.SoftDelete(enrollment.DeletedBy ?? "system");
            _context.Enrollments.Update(enrollment);
            return Task.CompletedTask;
        }
    }
}
