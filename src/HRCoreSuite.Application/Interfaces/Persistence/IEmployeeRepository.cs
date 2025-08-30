namespace HRCoreSuite.Application.Interfaces.Persistence;

public interface IEmployeeRepository
{
    Task<bool> IsEmployeeNumberUniqueAsync(string? employeeNumber, Guid? id, CancellationToken cancellationToken);
}