using TalentFlow.Domain.Entities;
using TalentFlow.Persistence;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly TalentFlowDbContext _db;

    public SubmissionRepository(TalentFlowDbContext db)
    {
        _db = db;
    }

    public async Task<Submission?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Submissions.FindAsync(new object[] { id }, ct);
    }
}