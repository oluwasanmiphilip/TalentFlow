using System;

namespace TalentFlow.Domain.Entities
{
    public class LearningTask
    {
        public Guid Id { get; set; }
        public Guid AssignedTo { get; set; }   // Learner/User ID
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public LearningTaskStatus Status { get; set; } = LearningTaskStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum LearningTaskStatus
    {
        Pending,
        InProgress,
        Completed
    }
}
