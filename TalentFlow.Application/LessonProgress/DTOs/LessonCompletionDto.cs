using System;

namespace TalentFlow.Application.LessonProgress.DTOs
{
    public class LessonCompletionDto
    {
        public DateTime CompletedAt { get; set; }
        public Guid? NextLessonId { get; set; }
        public decimal CoursePercentage { get; set; }
    }
}
