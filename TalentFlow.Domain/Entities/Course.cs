using TalentFlow.Domain.Common;


namespace TalentFlow.Domain.Entities
{
    public class Course : EntityBase
    {
        public Guid Id { get; private set; }
        public string Slug { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        private readonly List<Enrollment> _enrollments = new();
        public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

        private Course() { } // EF Core

        public Course(string title, string description)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Slug = GenerateSlug(title);

            // Raise domain-level event
            AddDomainEvent(new TalentFlow.Domain.Events.CourseCreatedDomainEvent(this));
        }

        public void Enroll(User user)
        {
            var enrollment = new Enrollment(user.Id, Id);
            _enrollments.Add(enrollment);

            AddDomainEvent(new TalentFlow.Domain.Events.CourseEnrollmentDomainEvent(enrollment, this, user));
        }

        private string GenerateSlug(string title) =>
            title.ToLower().Replace(" ", "-");
    }
}
