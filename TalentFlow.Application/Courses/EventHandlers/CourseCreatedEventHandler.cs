using MediatR;
using Microsoft.Extensions.Logging;
using TalentFlow.Application.Courses.Events;

namespace TalentFlow.Application.Courses.Handlers
{
    public class CourseCreatedEventHandler : INotificationHandler<CourseCreatedEvent>
    {
        private readonly ILogger<CourseCreatedEventHandler> _logger;

        public CourseCreatedEventHandler(ILogger<CourseCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(CourseCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Course created with ID {CourseId} at {OccurredOn}",
                notification.CourseId,
                notification.OccurredOn);

            // TODO: trigger onboarding, send notification, etc.

            return Task.CompletedTask;
        }
    }
}
