using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Courses.Commands;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Courses.Handlers
{
    public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, bool>
    {
        private readonly ICourseRepository _repo;

        public UpdateCourseHandler(ICourseRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task<bool> Handle(UpdateCourseCommand request, CancellationToken ct)
        {
            var course = await _repo.GetByIdAsync(request.Id, ct);
            if (course == null || course.IsDeleted) return false;

            course.UpdateDetails(
                request.Title,
                request.Description,
                request.ThumbnailUrl,
                request.InstructorId,
                request.DurationMinutes,
                request.Level,
                request.Price,
                request.Tags,
                request.UpdatedBy
            );

            await _repo.UpdateAsync(course, ct);
            return true;
        }

    }
}
