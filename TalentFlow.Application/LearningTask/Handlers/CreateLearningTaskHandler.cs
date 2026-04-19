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
//    public class CreateLearningTaskHandler : IRequestHandler<CreateLearningTaskCommand, LearningTaskDto>
//    {
//        private readonly ILearningTaskRepository _repository;
//        private readonly IUnitOfWork _unitOfWork;

//        public CreateLearningTaskHandler(ILearningTaskRepository repository, IUnitOfWork unitOfWork)
//        {
//            _repository = repository;
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<LearningTaskDto> Handle(CreateLearningTaskCommand request, CancellationToken cancellationToken)
//        {
//            //var task = new LearningTask
//            {
//                Id = Guid.NewGuid(),
//                AssignedTo = request.AssignedTo,
//                Title = request.Title,
//                Description = request.Description,
//                DueDate = request.DueDate,
//                Status = TaskStatus.Pending,
//                CreatedAt = DateTime.UtcNow
//            };

//            await _repository.AddAsync(task, cancellationToken);
//            await _unitOfWork.SaveChangesAsync(cancellationToken);

//            return new LearningTaskDto
//            {
//                Id = task.Id,
//                AssignedTo = task.AssignedTo,
//                Title = task.Title,
//                Description = task.Description,
//                DueDate = task.DueDate,
//                Status = task.Status
//            };
//        }
//    }
//}
