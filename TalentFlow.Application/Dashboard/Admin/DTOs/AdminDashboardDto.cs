namespace TalentFlow.Application.Dashboard.Admin.DTOs
{
    public class AdminDashboardDto
    {
        public string AdminId { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int PendingCourses { get; set; }
        public int ActiveCourses { get; set; }
        public List<string> Notifications { get; set; } = new();
    }
}
