using AutoMapper;
using HRCoreSuite.Application.DTOs.Position;
using HRCoreSuite.Domain;
using HRCoreSuite.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRCoreSuite.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PositionController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PositionResponseDto>>> GetPostions()
    {
        var positions = await _context.Positions.ToListAsync();
        var positionDtos = _mapper.Map<IEnumerable<PositionResponseDto>>(positions);
        return Ok(positionDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PositionResponseDto>> GetPositionById(Guid id)
    {
        var position = await _context.Positions.FindAsync(id);

        if (position == null)
        {
            return NotFound();
        }

        var positionDto = _mapper.Map<PositionResponseDto>(position);
        return Ok(positionDto);
    }

    [HttpPost]
    public async Task<ActionResult<PositionResponseDto>> CreatePosition(CreatePositionDto request)
    {
        var positonEntity = _mapper.Map<Position>(request);

        _context.Positions.Add(positonEntity);
        await _context.SaveChangesAsync();

        var positionDto = _mapper.Map<PositionResponseDto>(positonEntity);
        return CreatedAtAction(nameof(GetPositionById), new { id = positionDto.Id }, positionDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePosition(Guid id, UpdatePositionDto request)
    {
        var positonEntity = await _context.Positions.FindAsync(id);

        if (positonEntity == null)
        {
            return NotFound();
        }

        _mapper.Map(request, positonEntity);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePosition(Guid id)
    {
        var positonEntity = await _context.Positions.FindAsync(id);

        if (positonEntity == null)
        {
            return NotFound();
        }

        _context.Positions.Remove(positonEntity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}