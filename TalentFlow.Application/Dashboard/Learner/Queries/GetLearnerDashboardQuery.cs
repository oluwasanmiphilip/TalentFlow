using MediatR;
using TalentFlow.Application.Dashboard.Learner.DTOs;

namespace TalentFlow.Application.Dashboard.Learner.Queries
{
    public record GetLearnerDashboardQuery(string LearnerId) : IRequest<LearnerDashboardDto>;
}
