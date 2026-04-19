using System;
using TalentFlow.Application.Certificates.DTOs;
using TalentFlow.Application.Lessons.DTOs;
using TalentFlow.Application.Progresses.DTOs;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Mappings
{
    public static partial class EntityToDtoMapper
    {
        public static LessonDto ToDto(this Lesson lesson) => new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Content = lesson.Content,
            ContentUrl = lesson.ContentUrl,
            Order = lesson.Order,
            Duration = lesson.Duration,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt
        };

        public static ProgressDto ToDto(this LessonProgress progress) => new ProgressDto
        {
            LearnerId = progress.UserId,
            LessonId = progress.LessonId,
            CoursePercentage = progress.CoursePercentage,
            CompletedAt = progress.CompletedAt
        };
        public static CertificateDto ToDto(this Certificate certificate) => new CertificateDto
        {
            Id = certificate.Id,
            LearnerId = certificate.LearnerId,
            CourseId = certificate.CourseId,
            IssuedAt = certificate.IssuedAt,
            CertificateUrl = certificate.CertificateUrl
        };
    }
}
