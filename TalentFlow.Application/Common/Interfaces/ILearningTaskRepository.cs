//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using TalentFlow.Domain.Entities;

//namespace TalentFlow.Application.Common.Interfaces
//{
//    public interface ILearningTaskRepository
//    {
//        Task<LearningTaskStatus?> GetByIdAsync(Guid id, CancellationToken ct);
//        Task<List<LearningTask>> GetByUserAsync(Guid userId, CancellationToken ct);
//        Task AddAsync(LearningTaskStatus task, CancellationToken ct);
//        Task UpdateAsync(LearningTask task, CancellationToken ct);
//        Task DeleteAsync(Guid id, CancellationToken ct);
//    }
//}
