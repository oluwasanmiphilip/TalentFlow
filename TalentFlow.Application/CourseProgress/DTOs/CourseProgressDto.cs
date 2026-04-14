using System;

namespace TalentFlow.Application.CourseProgress.DTOs
{
    public class CourseProgressDto
    {
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
        public decimal Percentage { get; set; }
        public bool CertificateUnlocked { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
