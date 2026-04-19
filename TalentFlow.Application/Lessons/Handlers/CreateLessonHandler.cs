using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Mappings;
using TalentFlow.Application.Lessons.Commands;
using TalentFlow.Application.Lessons.DTOs;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Lessons.Handlers
{
    public class CreateLessonHandler : IRequestHandler<CreateLessonCommand, LessonDto>
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateLessonHandler(ILessonRepository lessonRepository, IUnitOfWork unitOfWork)
        {
            _lessonRepository = lessonRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<LessonDto> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
        {
            // ✅ Validate duration
            if (request.Duration.TotalHours <= 0)
                throw new ArgumentException("Duration must be greater than zero");
            if (request.Duration.TotalHours > 10)
                throw new ArgumentException("Duration exceeds maximum allowed length");

            // ✅ Validate ContentUrl (video/PDF/quiz)
            if (string.IsNullOrWhiteSpace(request.ContentUrl))
                throw new ArgumentException("ContentUrl is required for lesson playback");

            // ✅ Create Lesson entity
            var lesson = new Lesson(
                request.CourseId,
                request.Title,
                request.Content,
                request.ContentUrl,
                request.Order,
                request.Duration
            );

            await _lessonRepository.AddAsync(lesson, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ✅ Map Lesson → LessonDto
            return lesson.ToDto();
        }
    }
}
