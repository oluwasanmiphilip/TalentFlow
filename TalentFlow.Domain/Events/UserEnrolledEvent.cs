using System;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class UserEnrolledEvent : DomainEvent
    {
        public Enrollment Enrollment { get; }

        public UserEnrolledEvent(Enrollment enrollment)
        {
            Enrollment = enrollment ?? throw new ArgumentNullException(nameof(enrollment));
        }
    }
}
