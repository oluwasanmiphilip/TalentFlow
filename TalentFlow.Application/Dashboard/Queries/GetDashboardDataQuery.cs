// File Path: TalentFlow.Application/Dashboard/Queries/GetDashboardDataQuery.cs
using MediatR;
using TalentFlow.Application.Dashboard.DTOs;

namespace TalentFlow.Application.Dashboard.Queries
{
    public record GetDashboardDataQuery(string LearnerId) : IRequest<DashboardDto>;
}
