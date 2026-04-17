using TalentFlow.Domain.Entities;

public interface ILearnerRepository
{
    Task<IEnumerable<Course>> GetCoursesByLearnerIdAsync(Guid learnerId);
    Task<CourseProgress> GetCourseProgressAsync(Guid learnerId, Guid courseId);
    Task<IEnumerable<Notification>> GetNotificationsByLearnerIdAsync(Guid learnerId);
    
    Task<IEnumerable<User>> GetTeamMembersByCohortAsync(Guid learnerId);
}
