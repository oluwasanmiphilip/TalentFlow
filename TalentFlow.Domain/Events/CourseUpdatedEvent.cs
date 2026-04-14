using System;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class CourseUpdatedEvent : DomainEvent
    {
        public Course Course { get; }

        public CourseUpdatedEvent(Course course)
        {
            Course = course ?? throw new ArgumentNullException(nameof(course));
        }
    }
}
