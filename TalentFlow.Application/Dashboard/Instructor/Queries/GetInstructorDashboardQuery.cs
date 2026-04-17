using MediatR;
using TalentFlow.Application.Dashboard.Instructor.DTOs;

namespace TalentFlow.Application.Dashboard.Instructor.Queries
{
    public record GetInstructorDashboardQuery(string InstructorId) : IRequest<InstructorDashboardDto>;
}
