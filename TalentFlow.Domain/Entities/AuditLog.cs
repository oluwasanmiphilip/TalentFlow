using TalentFlow.Domain.Common;

namespace TalentFlow.Domain.Entities
{
    public class AuditLog : EntityBase
    {
        public Guid Id { get; private set; }
        public string EntityName { get; private set; }
        public string Action { get; private set; }
        public string PerformedBy { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Details { get; private set; }

        private AuditLog() { } // EF Core

        public AuditLog(string entityName, string action, string performedBy, string details)
        {
            Id = Guid.NewGuid();
            EntityName = entityName;
            Action = action;
            PerformedBy = performedBy;
            Details = details;
            Timestamp = DateTime.UtcNow;
        }
    }
}
