using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Courses.Commands;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;
using TalentFlow.Domain.Events;

namespace TalentFlow.Application.Courses.Handlers
{
    public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Guid>
    {
        private readonly ICourseRepository _repo;
        private readonly IMediator _mediator;   // ✅ add mediator

        public CreateCourseHandler(ICourseRepository repo, IMediator mediator)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken ct)
        {
            var course = new Course(
                request.Title,
                request.Description,
                request.Slug,
                request.ThumbnailUrl,
                request.InstructorId,
                request.DurationMinutes,
                request.Level,
                request.Price,
                request.Tags
            );

            await _repo.AddAsync(course, ct);

            var domainEvent = new CourseCreatedEvent(course);
            await _mediator.Publish(domainEvent, ct);

            return course.Id;
        }

    }
}
