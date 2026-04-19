using System;
using MediatR;
using TalentFlow.Application.LearningTask.DTOs;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.LearningTask.Commands
{
    public class UpdateLearningTaskCommand : IRequest<LearningTaskDto>
    {
        public Guid Id { get; }
        public string Title { get; }
        public string? Description { get; }
        public DateTime DueDate { get; }
        public LearningTaskStatus Status { get; }

        public UpdateLearningTaskCommand(Guid id, string title, string? description, DateTime dueDate, LearningTaskStatus status)
        {
            Id = id;
            Title = title;
            Description = description;
            DueDate = dueDate;
            Status = status;
        }
    }
}
