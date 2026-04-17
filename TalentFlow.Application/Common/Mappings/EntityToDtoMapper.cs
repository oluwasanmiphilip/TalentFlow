using System.Linq;
using TalentFlow.Domain.Entities;
using TalentFlow.Application.Assessments.DTOs;
using TalentFlow.Application.Progresses.DTOs;
using TalentFlow.Application.Lessons.DTOs;
using TalentFlow.Application.Teams.DTOs;
using TalentFlow.Application.Certificates.DTOs;
using TalentFlow.Application.Videos.DTOs;
using TalentFlow.Application.Notifications.DTOs;

namespace TalentFlow.Application.Common.Mappings
{
    public static class EntityToDtoMapper
    {
        public static AssessmentDto ToDto(this Assessment assessment)
        {
            return new AssessmentDto
            {
                Id = assessment.Id,
                CourseId = assessment.CourseId,
                Title = assessment.Title,
                Instructions = assessment.Instructions,
                CreatedAt = assessment.CreatedAt,
                UpdatedBy = assessment.UpdatedBy,
                UpdatedAt = assessment.UpdatedAt,
                DeletedBy = assessment.DeletedBy,
                DeletedAt = assessment.DeletedAt,
                IsDeleted = assessment.IsDeleted,
                Questions = assessment.Questions
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        Text = q.Text,
                        Answer = q.Answer
                    }).ToList()
            };
        }

        public static ProgressDto ToDto(this Progress progress) => new ProgressDto
        {
            Id = progress.Id,
            LearnerId = progress.LearnerId,
            CourseId = progress.CourseId,
            LessonId = progress.LessonId,
            PercentageCompleted = progress.PercentageCompleted,
            LastAccessed = progress.LastAccessed
        };

        public static LessonDto ToDto(this Lesson lesson) => new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Content = lesson.Content,
            Order = lesson.Order,
            Duration = lesson.Duration,
            CreatedAt = lesson.CreatedAt
        };

        public static TeamDto ToDto(this Team team) => new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            CreatedAt = team.CreatedAt,
            Members = team.Members.ToList()
        };

        public static CertificateDto ToDto(this Certificate certificate) => new CertificateDto
        {
            Id = certificate.Id,
            LearnerId = certificate.LearnerId,
            CourseId = certificate.CourseId,
            IssuedAt = certificate.IssuedAt,
            ExpiresAt = certificate.ExpiresAt,
            IssuedBy = certificate.IssuedBy
        };

        public static VideoDto ToDto(this Video video) => new VideoDto
        {
            Id = video.Id,
            LessonId = video.LessonId,
            Title = video.Title,
            Url = video.Url,
            Duration = video.Duration,
            Transcript = video.Transcript,
            CreatedAt = video.CreatedAt
        };

        public static NotificationDto ToDto(this Notification notification) => new NotificationDto
        {
            Id = notification.Id.ToString(),
            UserId = notification.UserId,
            Message = notification.Message,
            CreatedAt = notification.CreatedAt,
            IsDeleted = notification.IsDeleted,
            DeletedBy = notification.DeletedBy,
            DeletedAt = notification.DeletedAt
        };
    }
}