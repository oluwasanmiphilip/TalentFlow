using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Courses.DTOs;
using TalentFlow.Application.Courses.Mappings;

namespace TalentFlow.Application.Courses.Queries
{
    public class GetCoursesByLearnerHandler : IRequestHandler<GetCoursesByLearnerQuery, List<CourseDto>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;

        public GetCoursesByLearnerHandler(ICourseRepository courseRepository, IUserRepository userRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }

        public async Task<List<CourseDto>> Handle(GetCoursesByLearnerQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByLearnerIdAsync(request.LearnerId, cancellationToken);
            if (user is null) return new List<CourseDto>();

            var courses = await _courseRepository.GetByLearnerIdAsync(user.Id, cancellationToken);
            return courses.Select(c => c.ToDto()).ToList();
        }
    }
}
