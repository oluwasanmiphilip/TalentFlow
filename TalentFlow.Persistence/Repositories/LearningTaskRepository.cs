//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using TalentFlow.Application.LearningTask;
//using TalentFlow.Domain.Entities;

//namespace TalentFlow.Persistence.Repositories
//{
//    public class LearningTaskRepository : ILearningTaskRepository
//    {
//        private readonly TalentFlowDbContext _db;

//        public LearningTaskRepository(TalentFlowDbContext db)
//        {
//            _db = db;
//        }

//        public async Task<LearningTask?> GetByIdAsync(Guid id, CancellationToken ct)
//        {
//            return await _db.LearningTasks.FindAsync(new object[] { id }, ct);
//        }

//        public async Task<List<LearningTask>> GetByUserAsync(Guid userId, CancellationToken ct)
//        {
//            return await _db.LearningTasks
//                .Where(t => t.AssignedTo == userId)
//                .ToListAsync(ct);
//        }

//        public async Task AddAsync(LearningTask task, CancellationToken ct)
//        {
//            await _db.LearningTasks.AddAsync(task, ct);
//        }

//        public async Task UpdateAsync(LearningTask task, CancellationToken ct)
//        {
//            _db.LearningTasks.Update(task);
//            await Task.CompletedTask;
//        }

//        public async Task DeleteAsync(Guid id, CancellationToken ct)
//        {
//            var task = await _db.LearningTasks.FindAsync(new object[] { id }, ct);
//            if (task != null)
//            {
//                _db.LearningTasks.Remove(task);
//            }
//        }
//    }
//}
