using System;

namespace TalentFlow.Application.LearnersTask.DTOs
{
    public class CreateLearningTaskDto
    {
        public Guid AssignedTo { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}
