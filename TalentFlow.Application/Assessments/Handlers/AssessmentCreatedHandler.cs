using MediatR;
using Microsoft.Extensions.Logging;
using TalentFlow.Application.Assessments.Events;

namespace TalentFlow.Application.Assessments.Handlers
{
    public class AssessmentCreatedHandler : INotificationHandler<AssessmentCreatedEvent>
    {
        private readonly ILogger<AssessmentCreatedHandler> _logger;

        public AssessmentCreatedHandler(ILogger<AssessmentCreatedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(AssessmentCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Assessment created: Id={AssessmentId}, OccurredOn={OccurredOn}",
                notification.AssessmentId,
                notification.OccurredOn);

            // TODO: trigger downstream workflows (analytics, audit, etc.)
            return Task.CompletedTask;
        }
    }
}
