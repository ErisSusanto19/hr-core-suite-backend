namespace HRCoreSuite.Application.DTOs.Employee;

public class CreateEmployeeDto
{
    public string? EmployeeNumber { get; set; }
    public string? Name { get; set; }
    public DateOnly ContractStartDate { get; set; }
    public DateOnly ContractEndDate { get; set; }
    public Guid BranchId { get; set; }
    public Guid PositionId { get; set; }
}