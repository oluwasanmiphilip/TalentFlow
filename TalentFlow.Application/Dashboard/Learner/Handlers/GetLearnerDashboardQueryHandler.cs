using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Dashboard.Learner.Queries;

public class GetLearnerDashboardQueryHandler : IRequestHandler<GetLearnerDashboardQuery, LearnerDashboardDto>
{
    private readonly ICourseRepository _courseRepo;
    private readonly ICertificateRepository _certificateRepo;
    private readonly INotificationRepository _notificationRepo;

    public GetLearnerDashboardQueryHandler(
        ICourseRepository courseRepo,
        ICertificateRepository certificateRepo,
        INotificationRepository notificationRepo)
    {
        _courseRepo = courseRepo;
        _certificateRepo = certificateRepo;
        _notificationRepo = notificationRepo;
    }

    public async Task<LearnerDashboardDto> Handle(GetLearnerDashboardQuery request, CancellationToken cancellationToken)
    {
        var learnerGuid = Guid.Parse(request.LearnerId);

        var courses = await _courseRepo.GetCoursesByUserIdAsync(learnerGuid, cancellationToken);
        var certificates = await _certificateRepo.GetCertificatesByLearnerIdAsync(learnerGuid, cancellationToken);
        var notifications = await _notificationRepo.GetNotificationsByUserIdAsync(learnerGuid);

        return new LearnerDashboardDto
        {
            LearnerId = request.LearnerId,
            Courses = courses.Select(c => c.Title).ToList(),
            Certificates = certificates.Select(cert => cert.CourseId.ToString()).ToList(),
            Notifications = notifications.Select(n => n.Message).ToList()
        };
    }
}
