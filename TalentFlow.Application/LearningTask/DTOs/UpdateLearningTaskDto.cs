using System;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.LearningTask.DTOs
{
    public class UpdateLearningTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public LearningTaskStatus Status { get; set; }
    }
}
