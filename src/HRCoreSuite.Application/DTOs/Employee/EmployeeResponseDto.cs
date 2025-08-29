namespace HRCoreSuite.Application.DTOs.Employee;

public class EmployeeResponseDto
{
    public Guid Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateOnly ContractStartDate { get; set; }
    public DateOnly ContractEndDate { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
}