// File Path: TalentFlow.Application/Dashboard/Handlers/GetDashboardDataQueryHandler.cs
using MediatR;
using System.Runtime.ConstrainedExecution;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Dashboard.DTOs;
using TalentFlow.Application.Dashboard.Queries;

namespace TalentFlow.Application.Dashboard.Handlers
{
    public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, DashboardDto>
    {
        private readonly ICourseRepository _courseRepo;
        private readonly ICertificateRepository _certificateRepo;
        private readonly INotificationRepository _notificationRepo;

        public GetDashboardDataQueryHandler(
            ICourseRepository courseRepo,
            ICertificateRepository certificateRepo,
            INotificationRepository notificationRepo)
        {
            _courseRepo = courseRepo;
            _certificateRepo = certificateRepo;
            _notificationRepo = notificationRepo;
        }

        public async Task<DashboardDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            var learnerGuid = Guid.Parse(request.LearnerId);

            var courses = await _courseRepo.GetCoursesByUserIdAsync(learnerGuid);
            var certificates = await _certificateRepo.GetCertificatesByLearnerIdAsync(learnerGuid);
            var notifications = await _notificationRepo.GetNotificationsByUserIdAsync(learnerGuid);

            return new DashboardDto
            {
                LearnerId = request.LearnerId,
                Courses = courses.Select(c => c.Title).ToList(),
                Certificates = certificates.Select(cert => cert.CourseId.ToString()).ToList(), // fallback if no Title
                Notifications = notifications.Select(n => n.Message).ToList()
            };
        }

    }
}
