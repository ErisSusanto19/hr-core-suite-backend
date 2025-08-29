namespace HRCoreSuite.Application.Interfaces.Persistence;

public interface IBranchRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}