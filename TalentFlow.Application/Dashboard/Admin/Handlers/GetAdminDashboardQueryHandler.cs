using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Dashboard.Admin.DTOs;
using TalentFlow.Application.Dashboard.Admin.Queries;
using TalentFlow.Application.Dashboard.Events;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Dashboard.Admin.Handlers
{
    public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
    {
        private readonly ICourseRepository _courseRepo;
        private readonly INotificationRepository _notificationRepo;

        public GetAdminDashboardQueryHandler(
            ICourseRepository courseRepo,
            INotificationRepository notificationRepo)
        {
            _courseRepo = courseRepo;
            _notificationRepo = notificationRepo;
        }

        public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            var adminGuid = Guid.Parse(request.AdminId);

            var allCourses = await _courseRepo.GetAllAsync(cancellationToken);
            var notifications = await _notificationRepo.GetNotificationsByUserIdAsync(adminGuid);

            var totalUsers = allCourses.SelectMany(c => c.Enrollments).Select(e => e.UserId).Distinct().Count();
            var totalCourses = allCourses.Count;
            var pendingCourses = allCourses.Count(c => c.Status == "draft");
            var activeCourses = allCourses.Count(c => c.Status == "published");

            // Raise dashboard viewed event
            await _notificationRepo.AddAsync(new Notification(adminGuid, "Admin dashboard viewed"), cancellationToken);

            return new AdminDashboardDto
            {
                AdminId = request.AdminId,
                TotalUsers = totalUsers,
                TotalCourses = totalCourses,
                PendingCourses = pendingCourses,
                ActiveCourses = activeCourses,
                Notifications = notifications.Select(n => n.Message).ToList()
            };
        }
    }
}
