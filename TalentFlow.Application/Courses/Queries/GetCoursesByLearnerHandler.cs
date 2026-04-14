using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<List<CourseDto>> Handle(GetCoursesByLearnerQuery request, CancellationToken cancellationToken)
        {
            // request.LearnerId should be a string, not Guid
            var user = await _userRepository.GetByLearnerIdAsync(request.LearnerId, cancellationToken);
            if (user == null) return new List<CourseDto>();

            var courses = await _courseRepository.GetCoursesByUserIdAsync(user.Id, cancellationToken);
            return courses.Select(c => c.ToDto()).ToList();
        }
    }
}
