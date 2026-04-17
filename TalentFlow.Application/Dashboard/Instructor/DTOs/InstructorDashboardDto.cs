namespace TalentFlow.Application.Dashboard.Instructor.DTOs
{
    public class InstructorDashboardDto
    {
        public string InstructorId { get; set; } = string.Empty;
        public List<string> Courses { get; set; } = new();
        public List<string> PendingAssignments { get; set; } = new();
        public List<string> Notifications { get; set; } = new();
        public int ActiveLearnersCount { get; set; }
    }
}
