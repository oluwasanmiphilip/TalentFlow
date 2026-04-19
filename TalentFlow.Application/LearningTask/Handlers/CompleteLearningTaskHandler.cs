//using MediatR;
//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using TalentFlow.Application.Common.Interfaces;
//using TalentFlow.Application.LearningTask.Commands;
//using TalentFlow.Application.LearningTask.DTOs;
//using TalentFlow.Application.LearningTask.Repositories;
//using TalentFlow.Domain.Entities;

//namespace TalentFlow.Application.LearningTask.Handlers
//{
//    public class CompleteLearningTaskHandler : IRequestHandler<CompleteLearningTaskCommand, LearningTaskDto>
//    {
//        private readonly ILearningTaskRepository _repository;
//        private readonly IUnitOfWork _unitOfWork;

//        public CompleteLearningTaskHandler(ILearningTaskRepository repository, IUnitOfWork unitOfWork)
//        {
//            _repository = repository;
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<LearningTaskDto> Handle(CompleteLearningTaskCommand request, CancellationToken cancellationToken)
//        {
//            var task = await _repository.GetByIdAsync(request.Id, cancellationToken);
//            if (task == null) throw new InvalidOperationException("Task not found.");

//            task.Status = LearningTaskStatus.Completed;
//            task.UpdatedAt = DateTime.UtcNow;

//            await _repository.UpdateAsync(task, cancellationToken);
//            await _unitOfWork.SaveChangesAsync(cancellationToken);

//            return new LearningTaskDto
//            {
//                Id = task.Id,
//                AssignedTo = task.AssignedTo,
//                Title = task.Title,
//                //Description = task.Description,
//                DueDate = task.DueDate,
//                Status = task.Status
//            };
//        }
//    }
//}
