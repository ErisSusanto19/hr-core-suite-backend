using HRCoreSuite.Application.Interfaces.Persistence;
using FluentValidation;
using AutoMapper;
using HRCoreSuite.Domain;
using HRCoreSuite.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRCoreSuite.Application.DTOs.Employee;

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
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployees()
    {
        var employees = await _context.Employees
            .Include(e => e.Branch)
            .Include(e => e.Position)
            .ToListAsync();

        var employeeDtos = _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
        return Ok(employeeDtos);
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
            return NotFound();
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
            return NotFound();
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
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}