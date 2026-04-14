using System;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class EnrollmentWithdrawnEvent : DomainEvent
    {
        public Enrollment Enrollment { get; }

        public EnrollmentWithdrawnEvent(Enrollment enrollment)
        {
            Enrollment = enrollment ?? throw new ArgumentNullException(nameof(enrollment));
        }
    }
}
