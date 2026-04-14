using System;
using MediatR;
using TalentFlow.Application.Submissions.DTOs;

namespace TalentFlow.Application.Submissions.Queries
{
    public record GetSubmissionQuery(Guid SubmissionId) : IRequest<SubmissionDto>;
}
