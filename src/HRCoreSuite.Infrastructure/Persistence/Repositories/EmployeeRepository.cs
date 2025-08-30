using HRCoreSuite.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRCoreSuite.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmployeeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsEmployeeNumberUniqueAsync(string? employeeNumber, Guid? id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(employeeNumber))
        {
            return true;
        }
        
        return !await _dbContext.Employees
            .AnyAsync(e => e.EmployeeNumber == employeeNumber && e.Id != id, cancellationToken);
    }
}