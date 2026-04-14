using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.CourseProgress.DTOs;
using TalentFlow.Application.CourseProgress.Queries;
using TalentFlow.Application.CourseProgress.Repositories;

namespace TalentFlow.Application.CourseProgress.Handlers
{
    public class GetCourseProgressQueryHandler
        : IRequestHandler<GetCourseProgressQuery, CourseProgressDto?>
    {
        private readonly ICourseProgressRepository _courseRepo;

        public GetCourseProgressQueryHandler(ICourseProgressRepository courseRepo)
        {
            _courseRepo = courseRepo;
        }

        public async Task<CourseProgressDto?> Handle(GetCourseProgressQuery request, CancellationToken ct)
        {
            return await _courseRepo.GetProgressAsync(request.UserId, request.CourseId, ct);
        }
    }
}
