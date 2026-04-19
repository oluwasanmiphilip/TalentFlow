using System;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.LearningTask.DTOs
{
    public class LearningTaskDto
    {
        public Guid Id { get; set; }
        public Guid AssignedTo { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public LearningTaskStatus Status { get; set; }
    }
}
