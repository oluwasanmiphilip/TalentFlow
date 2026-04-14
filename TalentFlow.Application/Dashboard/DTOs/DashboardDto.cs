// File Path: TalentFlow.Application/Dashboard/DTOs/DashboardDto.cs
namespace TalentFlow.Application.Dashboard.DTOs
{
    public class DashboardDto
    {
        public string LearnerId { get; set; } = string.Empty;   
        public List<string> Courses { get; set; } = new();
        public List<string> Certificates { get; set; } = new();
        public List<string> Notifications { get; set; } = new();
    }
}
