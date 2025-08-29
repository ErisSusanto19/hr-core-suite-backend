namespace HRCoreSuite.Application.Interfaces.Persistence;

public interface IPositionRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}