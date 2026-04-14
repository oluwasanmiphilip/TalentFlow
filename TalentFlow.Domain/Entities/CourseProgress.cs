using System;

namespace TalentFlow.Domain.Entities
{
    public class CourseProgress
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }

        public decimal Percentage { get; set; }
        public bool CertificateUnlocked { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation
        public Course? Course { get; set; }
        public User? User { get; set; }
    }
}
