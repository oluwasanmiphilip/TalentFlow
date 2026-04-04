using MediatR;

namespace TalentFlow.Application.Courses.Commands
{
    public record CreateCourseCommand(string Title, string Description) : IRequest<string>;
}
