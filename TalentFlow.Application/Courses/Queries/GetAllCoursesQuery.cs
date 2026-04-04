using MediatR;
using TalentFlow.Application.Courses.DTOs;

namespace TalentFlow.Application.Courses.Queries
{
    public record GetAllCoursesQuery() : IRequest<List<CourseDto>>;
}
