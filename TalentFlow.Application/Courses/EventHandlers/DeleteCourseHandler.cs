using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Courses.Commands;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Courses.Handlers
{
    public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, bool>
    {
        private readonly ICourseRepository _repo;

        public DeleteCourseHandler(ICourseRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task<bool> Handle(DeleteCourseCommand request, CancellationToken ct)
        {
            var course = await _repo.GetByIdAsync(request.Id, ct);
            if (course == null || course.IsDeleted) return false;

            // Just call repository with deletedBy
            await _repo.SoftDeleteAsync(course, request.DeletedBy, ct);
            return true;
        }

    }
}
