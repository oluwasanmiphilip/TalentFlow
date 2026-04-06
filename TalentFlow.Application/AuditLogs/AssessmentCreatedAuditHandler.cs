using MediatR;
using TalentFlow.Domain.Events;
using TalentFlow.Domain.Entities;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common;
namespace TalentFlow.Application.AuditLogs
{
    public class AssessmentCreatedAuditHandler
        : INotificationHandler<DomainEventNotification<AssessmentCreatedDomainEvent>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogRepository _auditLogRepository;

        public AssessmentCreatedAuditHandler(IUnitOfWork unitOfWork, IAuditLogRepository auditLogRepository)
        {
            _unitOfWork = unitOfWork;
            _auditLogRepository = auditLogRepository;
        }

        public async Task Handle(DomainEventNotification<AssessmentCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var evt = notification.DomainEvent;

            var log = new AuditLog(
                entityName: nameof(Assessment),
                action: "Created",
                performedBy: "System", // or evt.Assessment.CreatedBy if you track user
                details: $"Assessment '{evt.Assessment.Title}' created at {evt.OccurredOn}"
            );

            await _auditLogRepository.AddAsync(log, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
