using System;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class CoursePublishedEvent : DomainEvent
    {
        public Course Course { get; }

        public CoursePublishedEvent(Course course)
        {
            Course = course ?? throw new ArgumentNullException(nameof(course));
        }
    }
}
