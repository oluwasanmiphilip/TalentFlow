using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Dashboard.Events;
using TalentFlow.Application.Dashboard.Instructor.DTOs;
using TalentFlow.Application.Dashboard.Instructor.Queries;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Dashboard.Instructor.Handlers
{
    public class GetInstructorDashboardQueryHandler : IRequestHandler<GetInstructorDashboardQuery, InstructorDashboardDto>
    {
        private readonly ICourseRepository _courseRepo;
        private readonly INotificationRepository _notificationRepo;

        public GetInstructorDashboardQueryHandler(
            ICourseRepository courseRepo,
            INotificationRepository notificationRepo)
        {
            _courseRepo = courseRepo;
            _notificationRepo = notificationRepo;
        }

        public async Task<InstructorDashboardDto> Handle(GetInstructorDashboardQuery request, CancellationToken cancellationToken)
        {
            var instructorGuid = Guid.Parse(request.InstructorId);

            var courses = await _courseRepo.GetCoursesByUserIdAsync(instructorGuid, cancellationToken);
            var notifications = await _notificationRepo.GetNotificationsByUserIdAsync(instructorGuid);

            // Pending assignments can be derived from assessments linked to courses
            var pendingAssignments = courses
                .SelectMany(c => c.Enrollments.Where(e => e.Role == "Instructor"))
                .Select(e => $"Assignments for course {e.CourseId}")
                .ToList();

            var activeLearnersCount = courses
                .SelectMany(c => c.Enrollments.Where(e => e.Role == "Learner"))
                .Count();

            // Raise dashboard viewed event
            await _notificationRepo.AddAsync(new Notification(instructorGuid, "Instructor dashboard viewed"), cancellationToken);

            return new InstructorDashboardDto
            {
                InstructorId = request.InstructorId,
                Courses = courses.Select(c => c.Title).ToList(),
                PendingAssignments = pendingAssignments,
                Notifications = notifications.Select(n => n.Message).ToList(),
                ActiveLearnersCount = activeLearnersCount
            };
        }
    }
}
