using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Courses.DTOs;
using TalentFlow.Application.Courses.Mappings;

namespace TalentFlow.Application.Courses.Queries
{
    public class GetAllCoursesHandler : IRequestHandler<GetAllCoursesQuery, List<CourseDto>>
    {
        private readonly ICourseRepository _courseRepository;

        public GetAllCoursesHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<List<CourseDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await _courseRepository.GetAllAsync(cancellationToken);
            return courses.Select(c => c.ToDto()).ToList();
        }
    }
}
