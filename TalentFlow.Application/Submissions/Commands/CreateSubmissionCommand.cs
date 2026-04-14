using System;
using MediatR;
using TalentFlow.Application.Submissions.DTOs;

namespace TalentFlow.Application.Submissions.Commands
{
    public record CreateSubmissionCommand(
        Guid AssignmentId,
        string? FilePath,
        string? Url,
        string? Text,
        string SubmittedBy
    ) : IRequest<SubmissionDto>;
}
