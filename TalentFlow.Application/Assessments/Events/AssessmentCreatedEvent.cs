using MediatR;

namespace TalentFlow.Application.Assessments.Events
{
    public class AssessmentCreatedEvent : INotification
    {
        public Guid AssessmentId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public AssessmentCreatedEvent(Guid assessmentId)
        {
            AssessmentId = assessmentId;
        }
    }
}
