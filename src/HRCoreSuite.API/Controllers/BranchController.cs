using AutoMapper;
using HRCoreSuite.Application.DTOs.Branch;
using HRCoreSuite.Domain;
using HRCoreSuite.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRCoreSuite.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BranchController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BranchResponseDto>>> GetBranches()
    {
        var branches = await _context.Branches.ToListAsync();
        var branchDtos = _mapper.Map<IEnumerable<BranchResponseDto>>(branches);
        return Ok(branchDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BranchResponseDto>> GetBranchById(Guid id)
    {
        var branch = await _context.Branches.FindAsync(id);

        if (branch == null)
        {
            return NotFound("Branch with ID " + id +  " not found.");
        }

        var branchDto = _mapper.Map<BranchResponseDto>(branch);
        return Ok(branchDto);
    }

    [HttpPost]
    public async Task<ActionResult<BranchResponseDto>> CreateBranch(CreateBranchDto request)
    {
        var branchEntity = _mapper.Map<Branch>(request);

        _context.Branches.Add(branchEntity);
        await _context.SaveChangesAsync();

        var branchResponseDto = _mapper.Map<BranchResponseDto>(branchEntity);

        return CreatedAtAction(nameof(GetBranchById), new { id = branchResponseDto.Id }, branchResponseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBranch(Guid id, UpdateBranchDto updateDto)
    {
        var branchEntity = await _context.Branches.FindAsync(id);

        if (branchEntity == null)
        {
            return NotFound("Branch with ID " + id +  " not found.");
        }

        _mapper.Map(updateDto, branchEntity);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBranch(Guid id)
    {
        var branchEntity = await _context.Branches.FindAsync(id);

        if (branchEntity == null)
        {
            return NotFound("Branch with ID " + id + " not found.");
        }

        _context.Branches.Remove(branchEntity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}