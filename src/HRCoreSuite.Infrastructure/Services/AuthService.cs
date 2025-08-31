using HRCoreSuite.Application.DTOs.Auth;
using HRCoreSuite.Application.Interfaces.Infrastructure;
using HRCoreSuite.Domain;
using HRCoreSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace HRCoreSuite.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == loginRequest.UserName);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
        {
            return null;
        }

        var roles = user.UserRoles
            .Where(ur => ur.Role != null)
            .Select(ur => ur.Role!.Name)
            .ToList();
        var token = GenerateJwtToken(user, roles);

        return new AuthResponseDto
        {
            UserName = user.UserName,
            Email = user.Email,
            Roles = roles,
            Token = token
        };
    }

    public async Task<bool> RegisterAsync(RegisterRequestDto registerRequest)
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.UserName == registerRequest.UserName || u.Email == registerRequest.Email);
        if (userExists)
        {
            return false;
        }

        var user = new User
        {
            UserName = registerRequest.UserName!,
            Email = registerRequest.Email!,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
            EmployeeId = registerRequest.EmployeeId
        };

        await _dbContext.Users.AddAsync(user);

        foreach (var roleId in registerRequest.RoleIds)
        {
            var userRole = new UserRole { UserId = user.Id, RoleId = roleId };
            await _dbContext.UserRoles.AddAsync(userRole);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    private string GenerateJwtToken(User user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.UserName),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(1);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}