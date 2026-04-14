using System;
using MediatR;
using TalentFlow.Application.LessonProgress.DTOs;

namespace TalentFlow.Application.LearningProgress.Commands
{
    public record MarkLessonCompleteCommand(Guid LessonId, Guid UserId) : IRequest<LessonCompletionDto>;
}
