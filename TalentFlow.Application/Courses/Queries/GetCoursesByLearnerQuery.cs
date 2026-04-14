using MediatR;
using TalentFlow.Application.Courses.DTOs;

namespace TalentFlow.Application.Courses.Queries
{
    public class GetCoursesByLearnerQuery : IRequest<List<CourseDto>>
    {
        public string LearnerId { get; }

        public GetCoursesByLearnerQuery(string learnerId)
        {
            LearnerId = learnerId;
        }
    }

}
