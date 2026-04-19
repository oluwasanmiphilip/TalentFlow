using System;

namespace TalentFlow.Application.LeanersProgress.DTOs
{
    public class LeanerCompletionDto
    {
        public DateTime CompletedAt { get; set; }
        public Guid? NextLessonId { get; set; }
        public decimal CoursePercentage { get; set; }
    }
}
