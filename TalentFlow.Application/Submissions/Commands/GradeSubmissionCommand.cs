using System;
using MediatR;

namespace TalentFlow.Application.Submissions.Commands
{
    public record GradeSubmissionCommand(
        Guid SubmissionId,
        decimal Score,
        string? InstructorComment,
        string GradedBy
    ) : IRequest<bool>;
}
