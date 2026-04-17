namespace TalentFlow.Application.Dashboard.Learner.DTOs
{
    public class LearnerDashboardDto
    {
        public string LearnerId { get; set; } = string.Empty;
        public List<string> Courses { get; set; } = new();
        public List<string> Certificates { get; set; } = new();
        public List<string> Notifications { get; set; } = new();
        public int ProgressPercentage { get; set; }
    }
}
