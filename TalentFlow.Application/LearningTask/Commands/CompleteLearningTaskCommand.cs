using System;
using MediatR;
using TalentFlow.Application.LearningTask.DTOs;

namespace TalentFlow.Application.LearningTask.Commands
{
    public class CompleteLearningTaskCommand : IRequest<LearningTaskDto>
    {
        public Guid Id { get; }

        public CompleteLearningTaskCommand(Guid id)
        {
            Id = id;
        }
    }
}
