using System;
using MediatR;

namespace TalentFlow.Application.LessonProgress.Commands
{
    public record UpdateVideoPositionCommand(Guid LessonId, Guid UserId, int VideoPositionSeconds)
        : IRequest<bool>;
}
