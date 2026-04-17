public class LearnerDashboardDto
{
    public IEnumerable<PendingActionDto> PendingActions { get; set; }
    public ContinueLearningDto ContinueLearning { get; set; }
    public IEnumerable<TeamMemberDto> MyTeam { get; set; }
    public ProgressRingDto ProgressRing { get; set; }
    public IEnumerable<NotificationDto> RecentNotifications { get; set; }
    public string LearnerId { get; internal set; }
    public List<string> Courses { get; internal set; }
    public List<string> Certificates { get; internal set; }
    public List<string> Notifications { get; internal set; }
}

public class PendingActionDto
{
    public string Title { get; set; }
    public string Type { get; set; } // e.g. "Lesson", "Assessment"
    public DateTime DueDate { get; set; }
}

public class ContinueLearningDto
{
    public string CourseId { get; set; }
    public string CourseTitle { get; set; }
    public string LessonId { get; set; }
    public string LessonTitle { get; set; }
    public int Progress { get; set; } // percentage
}

public class TeamMemberDto
{
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; } // e.g. "Peer", "Mentor"
}

public class ProgressRingDto
{
    public int CompletedCourses { get; set; }
    public int TotalCourses { get; set; }
    public int OverallProgressPercentage { get; set; }
}

public class NotificationDto
{
    public string Id { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; internal set; }
    public Guid UserId { get; internal set; }
    public bool IsDeleted { get; internal set; }
    public string? DeletedBy { get; internal set; }
    public DateTime? DeletedAt { get; internal set; }
}
