using HRCoreSuite.Application.Interfaces.Persistence;
using FluentValidation;
using AutoMapper;
using HRCoreSuite.Domain;
using HRCoreSuite.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRCoreSuite.Application.DTOs.Employee;
using HRCoreSuite.Application.DTOs.Common;
using System.Net;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;

namespace HRCoreSuite.API.Controllers;

[Authorize]
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
            return NotFound("Employee with ID " + id + " not found.");
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
            return NotFound("Employee with ID " + id + " not found.");
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
            return NotFound("Employee with ID " + id + " not found.");
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
    
    [HttpPost("upload")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UploadEmployees(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Tidak ada file yang diunggah.");
        }

        var successCount = 0;
        var failCount = 0;
        var errors = new List<string>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);

            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                if (worksheet == null)
                {
                    return BadRequest("File Excel tidak berisi worksheet.");
                }

                var rows = worksheet.RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    var rowNumber = row.RowNumber();
                    try
                    {

                        var employeeNumber = row.Cell(1).GetValue<string>().Trim();
                        var name = row.Cell(2).GetValue<string>().Trim();

                        var contractStartDateVal = row.Cell(3).GetValue<DateTime>();
                        var contractEndDateVal = row.Cell(4).GetValue<DateTime>();

                        var branchIdStr = row.Cell(5).GetValue<string>().Trim();
                        var positionIdStr = row.Cell(6).GetValue<string>().Trim();

                        if (string.IsNullOrEmpty(employeeNumber))
                        {
                           errors.Add($"Baris {rowNumber}: NIP kosong, baris dilewati.");
                           failCount++;
                           continue;
                        }

                        var existingEmployee = await _context.Employees
                                                   .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);
                        
                        var employeeData = new 
                        {
                            EmployeeNumber = employeeNumber,
                            Name = name,
                            ContractStartDate = DateOnly.FromDateTime(contractStartDateVal),
                            ContractEndDate = DateOnly.FromDateTime(contractEndDateVal),
                            BranchId = Guid.Parse(branchIdStr),
                            PositionId = Guid.Parse(positionIdStr)
                        };
                        
                        if (existingEmployee != null)
                        {
                            existingEmployee.Name = employeeData.Name;
                            existingEmployee.ContractStartDate = employeeData.ContractStartDate;
                            existingEmployee.ContractEndDate = employeeData.ContractEndDate;
                            existingEmployee.BranchId = employeeData.BranchId;
                            existingEmployee.PositionId = employeeData.PositionId;
                        }
                        else
                        {
                            var newEmployee = new Employee
                            {
                                EmployeeNumber = employeeData.EmployeeNumber,
                                Name = employeeData.Name,
                                ContractStartDate = employeeData.ContractStartDate,
                                ContractEndDate = employeeData.ContractEndDate,
                                BranchId = employeeData.BranchId,
                                PositionId = employeeData.PositionId
                            };
                            _context.Employees.Add(newEmployee);
                        }
                        
                        successCount++;
                    }
                    catch (DbUpdateException ex)
                    {
                        failCount++;
                        var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                        errors.Add($"Baris {rowNumber}: Error Database - {innerExceptionMessage}");
                        _context.ChangeTracker.Clear();
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        errors.Add($"Baris {rowNumber}: Terjadi error - {ex.Message}");
                    }
                }
            }
        }
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new 
            {
                Message = "Gagal menyimpan data ke database. Seluruh transaksi dibatalkan.",
                Detail = ex.InnerException?.Message ?? ex.Message
            });
        }

        return Ok(new
        {
            Message = "Proses unggah file selesai.",
            SuccessfulRows = successCount,
            FailedRows = failCount,
            Errors = errors
        });
    }
}