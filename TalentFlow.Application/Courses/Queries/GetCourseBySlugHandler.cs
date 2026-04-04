using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Courses.DTOs;
using TalentFlow.Application.Courses.Mappings;

namespace TalentFlow.Application.Courses.Queries
{
    public class GetCourseBySlugHandler : IRequestHandler<GetCourseBySlugQuery, CourseDto?>
    {
        private readonly ICourseRepository _courseRepository;

        public GetCourseBySlugHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<CourseDto?> Handle(GetCourseBySlugQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetBySlugAsync(request.Slug, cancellationToken);
            return course?.ToDto();
        }
    }
}
