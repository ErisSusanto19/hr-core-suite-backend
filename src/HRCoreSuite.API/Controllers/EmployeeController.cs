using HRCoreSuite.Application.Interfaces.Persistence;
using FluentValidation;
using AutoMapper;
using HRCoreSuite.Domain;
using HRCoreSuite.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRCoreSuite.Application.DTOs.Employee;
using HRCoreSuite.Application.DTOs.Common;

namespace HRCoreSuite.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IBranchRepository _branchRepository;
    private readonly IPositionRepository _positionRepository;

    public EmployeeController(
        ApplicationDbContext context,
        IMapper mapper,
        IBranchRepository branchRepository,
        IPositionRepository positionRepository)
    {
        _context = context;
        _mapper = mapper;
        _branchRepository = branchRepository;
        _positionRepository = positionRepository;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<EmployeeResponseDto>>> GetEmployees(
        [FromQuery] EmployeeQueryParameters queryParams)
    {

        if (queryParams == null)
        {
            return BadRequest("Query parameters are required.");
        }

        var query = _context.Employees
            .Include(e => e.Branch)
            .Include(e => e.Position)
            .AsQueryable();

        if (queryParams.BranchId.HasValue)
        {
            query = query.Where(e => e.BranchId == queryParams.BranchId.Value);
        }

        if (queryParams.PositionId.HasValue)
        {
            query = query.Where(e => e.PositionId == queryParams.PositionId.Value);
        }

        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            query = query.Where(e => 
                e.Name.Contains(queryParams.Search) || 
                e.EmployeeNumber.Contains(queryParams.Search));
        }

        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortOrder = queryParams.SortOrder ?? "asc";

            if (queryParams.SortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                query = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(e => e.Name)
                    : query.OrderBy(e => e.Name);
            }
            else if (queryParams.SortBy.Equals("contractEndDate", StringComparison.OrdinalIgnoreCase))
            {
                query = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(e => e.ContractEndDate)
                    : query.OrderBy(e => e.ContractEndDate);
            }
        }
        else
        {
            query = query.OrderBy(e => e.Name);
        }

        var totalItems = await query.CountAsync();

        var employees = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        var employeeDtos = _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);

        var pagedResponse = new PagedResponse<EmployeeResponseDto>(
            employeeDtos, 
            queryParams.Page, 
            queryParams.PageSize, 
            totalItems);

        return Ok(pagedResponse);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetEmployeeById(Guid id)
    {
        var employee = await _context.Employees
            .Include(e => e.Branch)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound("Employee with ID " + id +  " not found.");
        }

        var employeeDto = _mapper.Map<EmployeeResponseDto>(employee);
        return Ok(employeeDto);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee(
        [FromServices] IValidator<CreateEmployeeDto> validator,
        CreateEmployeeDto request)
    {

        var validationResult = await validator.ValidateAsync(request, options => options.IncludeRuleSets("async"));

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
        var employeeEntity = _mapper.Map<Employee>(request);

        _context.Employees.Add(employeeEntity);
        await _context.SaveChangesAsync();

        var createdEmployee = await _context.Employees
            .Include(e => e.Branch)
            .Include(e => e.Position)
            .FirstAsync(e => e.Id == employeeEntity.Id);

        var employeeResponseDto = _mapper.Map<EmployeeResponseDto>(createdEmployee);

        return CreatedAtAction(nameof(GetEmployeeById), new { id = employeeResponseDto.Id }, employeeResponseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(
        Guid id,
        UpdateEmployeeDto updateDto,
        [FromServices] IEmployeeRepository employeeRepo,
        [FromServices] IBranchRepository branchRepo,
        [FromServices] IPositionRepository positionRepo)
    {
        var employeeEntity = await _context.Employees.FindAsync(id);
        if (employeeEntity == null)
        {
            return NotFound("Employee with ID " + id +  " not found.");
        }

        if (!await employeeRepo.IsEmployeeNumberUniqueAsync(updateDto.EmployeeNumber, id, CancellationToken.None))
        {
            ModelState.AddModelError(nameof(updateDto.EmployeeNumber), "NIP sudah terdaftar.");
        }

        if (!await branchRepo.ExistsAsync(updateDto.BranchId, CancellationToken.None))
        {
            ModelState.AddModelError(nameof(updateDto.BranchId), "Cabang yang dipilih tidak valid atau tidak ditemukan.");
        }

        if (!await positionRepo.ExistsAsync(updateDto.PositionId, CancellationToken.None))
        {
            ModelState.AddModelError(nameof(updateDto.PositionId), "Jabatan yang dipilih tidak valid atau tidak ditemukan.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(updateDto, employeeEntity);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<EmployeeResponseDto>> DeleteEmployee(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound("Employee with ID " + id +  " not found.");
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpGet("expiring-contracts")]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetExpiringContracts(
        [FromQuery] int daysUntilExpiry = 30)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var targetDate = today.AddDays(daysUntilExpiry);

        var expiringEmployees = await _context.Employees
            .Include(e => e.Branch)
            .Include(e => e.Position)
            .Where(e => e.ContractEndDate >= today && e.ContractEndDate <= targetDate)
            .OrderBy(e => e.ContractEndDate)
            .ToListAsync();

        var employeeDtos = _mapper.Map<IEnumerable<EmployeeResponseDto>>(expiringEmployees);
        return Ok(employeeDtos);
    }
}