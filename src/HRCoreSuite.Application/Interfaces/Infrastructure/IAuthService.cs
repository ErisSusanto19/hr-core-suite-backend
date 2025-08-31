using HRCoreSuite.Application.DTOs.Auth;

namespace HRCoreSuite.Application.Interfaces.Infrastructure;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginRequest);
    Task<bool> RegisterAsync(RegisterRequestDto registerRequest);
}