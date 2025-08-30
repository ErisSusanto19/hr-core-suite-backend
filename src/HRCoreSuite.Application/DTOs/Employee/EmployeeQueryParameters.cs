using Microsoft.AspNetCore.Mvc;

namespace HRCoreSuite.Application.DTOs.Employee;

public class EmployeeQueryParameters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "pageSize")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    [FromQuery(Name = "search")]
    public string? Search { get; set; }

    [FromQuery(Name = "branchId")]
    public Guid? BranchId { get; set; }

    [FromQuery(Name = "positionId")]
    public Guid? PositionId { get; set; }

    [FromQuery(Name = "sortBy")]
    public string? SortBy { get; set; }

    [FromQuery(Name = "sortOrder")]
    public string? SortOrder { get; set; } = "asc";
}