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
        public static CourseDto ToDto(this Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Slug = course.Slug,
                ThumbnailUrl = course.ThumbnailUrl,
                InstructorId = course.InstructorId,
                DurationMinutes = course.DurationMinutes,
                Level = course.Level,
                Price = course.Price,
                Tags = course.Tags.ToList(),
                Rating = course.Rating,

                CreatedAt = course.CreatedAt,
                UpdatedBy = course.UpdatedBy,
                UpdatedAt = course.UpdatedAt,
                DeletedBy = course.DeletedBy,
                DeletedAt = course.DeletedAt,
                IsDeleted = course.IsDeleted,

                Enrollments = course.Enrollments
                    .Select(e => new EnrollmentDto
                    {
                        Id = e.Id,
                        CourseId = e.CourseId,
                        UserId = e.UserId,
                        Role = e.Role,
                        Status = e.Status,
                        UpdatedBy = e.UpdatedBy,
                        UpdatedAt = e.UpdatedAt,
                        DeletedBy = e.DeletedBy,
                        DeletedAt = e.DeletedAt,
                        IsDeleted = e.IsDeleted
                    })
                    .ToList()
            };
        }

    }
    
}
