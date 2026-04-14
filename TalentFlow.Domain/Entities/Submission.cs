using System;

namespace TalentFlow.Domain.Entities
{
    public class Submission
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }   // links to Assessment/Assignment
        public string ReferenceNumber { get; set; } = string.Empty;

        // Submission content
        public string? FilePath { get; set; }
        public string? Url { get; set; }
        public string? Text { get; set; }

        public string Status { get; set; } = "submitted";
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string SubmittedBy { get; set; } = string.Empty;

        // Grading fields
        public decimal? Score { get; set; }
        public string? InstructorComment { get; set; }

        // Navigation
        public Assessment? Assessment { get; set; }
    }
}
