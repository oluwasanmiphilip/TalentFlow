using MediatR;

namespace TalentFlow.Application.Dashboard.Events
{
    public record DashboardViewedEvent(string UserId, string Role) : INotification;
}
