using HRCoreSuite.Application.Interfaces.Persistence;
using HRCoreSuite.Domain;
using Microsoft.EntityFrameworkCore;

namespace HRCoreSuite.Infrastructure.Persistence.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PositionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Positions.AnyAsync(p => p.Id == id, cancellationToken);
    }
}