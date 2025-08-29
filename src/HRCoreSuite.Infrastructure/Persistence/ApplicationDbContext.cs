using HRCoreSuite.Domain;
using Microsoft.EntityFrameworkCore;

namespace HRCoreSuite.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Position> Positions { get; set; }

}