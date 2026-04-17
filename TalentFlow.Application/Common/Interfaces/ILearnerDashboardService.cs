using TalentFlow.Application.CourseProgress.DTOs;

public interface ILearnerDashboardService
{
    Task<LearnerDashboardDto> GetLearnerDashboardAsync(Guid learnerId);
    Task<CourseProgressDto> GetCourseProgressAsync(Guid learnerId, Guid courseId);
    Task<IEnumerable<NotificationDto>> GetNotificationsAsync(Guid learnerId);
}
