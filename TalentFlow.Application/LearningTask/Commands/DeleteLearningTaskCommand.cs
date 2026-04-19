using System;
using MediatR;

namespace TalentFlow.Application.LearningTask.Commands
{
    public class DeleteLearningTaskCommand : IRequest<bool>
    {
        public Guid Id { get; }

        public DeleteLearningTaskCommand(Guid id)
        {
            Id = id;
        }
    }
}
