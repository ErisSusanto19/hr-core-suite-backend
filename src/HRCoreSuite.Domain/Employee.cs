namespace HRCoreSuite.Domain;

public class Employee
{
    public Guid Id { get; set; }
    public required string EmployeeNumber { get; set; }
    public required string Name { get; set; }
    public DateOnly ContractStartDate { get; set; }
    public DateOnly ContractEndDate { get; set; }
    public Guid BranchId { get; set; }
    public Guid PositionId { get; set; }
    public Branch? Branch { get; set; }
    public Position? Position { get; set; }
}