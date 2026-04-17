using System.ComponentModel.DataAnnotations.Schema;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Events;


namespace TalentFlow.Domain.Entities
{
    [Table("assessment")] // matches EF query
    public class Assessment : EntityBase
    {
        public Guid Id { get; private set; }
        public Guid CourseId { get; private set; }   // Link to Course
        public string Title { get; private set; } = string.Empty;
        public string Instructions { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public Guid InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public string Status { get; set; } = "pending";

        // Audit fields
        public string? UpdatedBy { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string? DeletedBy { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private readonly List<Question> _questions = new();
        public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

        private Assessment() { } // EF Core

        public Assessment(Guid courseId, string title, string instructions)
        {
            Id = Guid.NewGuid();
            CourseId = courseId;
            Title = title;
            Instructions = instructions;
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;

            AddDomainEvent(new AssessmentCreatedDomainEvent(this));
        }

        public void AddQuestion(string text, string answer)
        {
            var question = new Question(Id, text, answer);
            _questions.Add(question);

            AddDomainEvent(new QuestionAddedDomainEvent(this, question));
        }

        public void UpdateDetails(string title, string instructions, string updatedBy)
        {
            Title = title;
            Instructions = instructions;
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete(string deletedBy)
        {
            IsDeleted = true;
            DeletedBy = deletedBy;
            DeletedAt = DateTime.UtcNow;
        }
    }
}
