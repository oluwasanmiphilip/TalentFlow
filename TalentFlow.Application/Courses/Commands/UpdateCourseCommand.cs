using System;
using MediatR;

namespace TalentFlow.Application.Courses.Commands
{
    public record UpdateCourseCommand(
    Guid Id,
    string Title,
    string Description,
    string ThumbnailUrl,
    Guid InstructorId,
    int DurationMinutes,
    string Level,
    decimal Price,
    List<string> Tags,
    string UpdatedBy
) : IRequest<bool>;

}