using HRCoreSuite.Application.DTOs.Auth;
using HRCoreSuite.Application.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace HRCoreSuite.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var authResponse = await _authService.LoginAsync(request);

        if (authResponse == null)
        {
            return Unauthorized("Username atau password salah.");
        }

        return Ok(authResponse);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var isSuccessful = await _authService.RegisterAsync(request);

        if (!isSuccessful)
        {
            return BadRequest("Username atau email sudah terdaftar.");
        }

        return Ok(new { Message = "Registrasi berhasil." });
    }
}