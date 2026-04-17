// TalentFlow.Application.Common.Interfaces
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IInstructorRepository
    {
        Task AddAsync(Instructor instructor, CancellationToken cancellationToken);
        Task<Instructor?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task UpdateAsync(Instructor instructor, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);

        // Optional: add queries you need, e.g. GetByCourseIdAsync
        Task<List<Instructor>> GetAllAsync(CancellationToken cancellationToken);


        Task<IEnumerable<Course>> GetCoursesByInstructorIdAsync(Guid instructorId);
        Task<IEnumerable<Assessment>> GetPendingAssignmentsAsync(Guid instructorId);
        
        Task<IEnumerable<Notification>> GetNotificationsByInstructorIdAsync(Guid instructorId);
    }
}
