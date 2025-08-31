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

        var hrAdminRoleId = Guid.Parse("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d");
        var superAdminRoleId = Guid.Parse("acc5645b-f080-4011-8c98-d7efa3032f52");
        var adminUserId = Guid.Parse("0b5c1848-fdd1-47bd-a3bf-ccb55ef297ef");

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = hrAdminRoleId, Name = "HR_Admin" },
            new Role { Id = superAdminRoleId, Name = "Super_Admin" }
        );

        // var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        var adminPasswordHash = "$2a$11$zeibt7oGO6Gl8YgPRSXaBuvGLGMfexe9DD2fwR9DsvwM0FOVmE0xu";

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