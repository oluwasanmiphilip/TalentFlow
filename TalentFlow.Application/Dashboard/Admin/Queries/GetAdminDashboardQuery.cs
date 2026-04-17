using MediatR;
using TalentFlow.Application.Dashboard.Admin.DTOs;

namespace TalentFlow.Application.Dashboard.Admin.Queries
{
    public record GetAdminDashboardQuery(string AdminId) : IRequest<AdminDashboardDto>;
}
