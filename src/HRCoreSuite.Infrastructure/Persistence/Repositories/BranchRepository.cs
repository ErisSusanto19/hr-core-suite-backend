using HRCoreSuite.Application.Interfaces.Persistence;
using HRCoreSuite.Domain;
using Microsoft.EntityFrameworkCore;

namespace HRCoreSuite.Infrastructure.Persistence.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BranchRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Branches.AnyAsync(b => b.Id == id, cancellationToken);
    }
}