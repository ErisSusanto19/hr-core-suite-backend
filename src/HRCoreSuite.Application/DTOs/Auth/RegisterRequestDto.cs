namespace HRCoreSuite.Application.DTOs.Auth;

public class RegisterRequestDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public Guid EmployeeId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}