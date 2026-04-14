using System;
using System.Collections.Generic;

namespace TalentFlow.Application.Submissions.DTOs
{
    public class SubmissionDto
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "submitted";
        public DateTime SubmittedAt { get; set; }
        public string SubmittedBy { get; set; } = string.Empty;

        // Submission content
        public string? FilePath { get; set; }
        public string? Url { get; set; }
        public string? Text { get; set; }

        // Grading fields
        public decimal? Score { get; set; }
        public Dictionary<string, decimal>? RubricScores { get; set; }
        public string? InstructorComment { get; set; }
    }
}
