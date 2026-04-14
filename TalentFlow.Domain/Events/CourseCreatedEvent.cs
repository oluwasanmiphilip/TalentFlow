using System;
using MediatR;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class CourseCreatedEvent : DomainEvent, INotification
    {
        public Course Course { get; }

        public CourseCreatedEvent(Course course)
        {
            Course = course ?? throw new ArgumentNullException(nameof(course));
        }
    }
}
