using MediatR;
using TalentFlow.Application.Courses.DTOs;

namespace TalentFlow.Application.Courses.Queries
{
    public record GetCoursesByLearnerQuery(string LearnerId) : IRequest<List<CourseDto>>;
}
