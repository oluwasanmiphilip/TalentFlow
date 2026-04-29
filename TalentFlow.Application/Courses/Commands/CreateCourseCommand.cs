// Application/Courses/Commands/CreateCourseCommand.cs
using System;
using MediatR;

namespace TalentFlow.Application.Courses.Commands
{
    public record CreateCourseCommand(
    string Title,
    string Description,
    string Slug,
    string ThumbnailUrl,
    Guid InstructorId,
    int DurationMinutes,
    string Level,
    decimal Price,
    List<string> Tags
) : IRequest<Guid>;
}
