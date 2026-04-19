using System;
using TalentFlow.Domain.Entities;
using TalentFlow.Application.Progresses.DTOs;

namespace TalentFlow.Application.Common.Mappings
{
    public static partial class DtoToEntityMapper
    {
        public static LessonProgress ToEntity(this ProgressDto dto)
        {
            var progress = new LessonProgress(dto.LearnerId, dto.LessonId);

            // ✅ Restore state from DTO
            if (dto.VideoPositionSeconds > 0 || dto.CoursePercentage > 0)
            {
                progress.UpdateProgress(dto.CoursePercentage, TimeSpan.FromSeconds(dto.VideoPositionSeconds));
            }

            if (dto.CompletedAt.HasValue)
            {
                progress.MarkComplete();
            }

            return progress;
        }
    }
}
