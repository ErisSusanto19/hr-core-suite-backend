using HRCoreSuite.Application.Constants;
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

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany()
            .HasForeignKey(ur => ur.RoleId);

        var hrAdminRoleId = Guid.Parse(RoleConstants.HrAdminId);
        var superAdminRoleId = Guid.Parse(RoleConstants.SuperAdminId);
        var adminUserId = Guid.Parse(UserConstants.SuperAdminId);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = hrAdminRoleId, Name = "HR_Admin" },
            new Role { Id = superAdminRoleId, Name = "Super_Admin" }
        );

        var adminPasswordHash = UserConstants.SuperAdminPasswordHash;

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                UserName = "admin",
                Email = "admin@hrcoresuite.com",
                PasswordHash = adminPasswordHash,
                EmployeeId = null
            }
        );
        
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = adminUserId, RoleId = superAdminRoleId }
        );
    }

}