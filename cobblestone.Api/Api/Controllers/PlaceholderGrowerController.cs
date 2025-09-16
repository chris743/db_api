using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Domain;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/placeholdergrower")]
[ApiVersion("1.0")]
[Authorize]
public class PlaceholderGrowerController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<PlaceholderGrowerController> _logger;

    public PlaceholderGrowerController(AppDbContext db, ILogger<PlaceholderGrowerController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlaceholderGrowerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PlaceholderGrowerDto>>> List(
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 100,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var query = _db.PlaceholderGrowers.AsNoTracking();

        // Filter by active status
        if (isActive.HasValue)
        {
            query = query.Where(x => x.is_active == isActive.Value);
        }

        // Search by grower name or commodity name
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x => 
                x.grower_name.Contains(search) || 
                x.commodity_name.Contains(search));
        }

        var rows = await query
            .OrderByDescending(x => x.created_at)
            .Skip(skip)
            .Take(take)
            .Select(x => new PlaceholderGrowerDto(
                x.id,
                x.grower_name,
                x.commodity_name,
                x.created_at,
                x.updated_at,
                x.is_active,
                x.notes
            ))
            .ToListAsync(ct);

        return Ok(rows);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PlaceholderGrowerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlaceholderGrowerDto>> Get(Guid id, CancellationToken ct)
    {
        var entity = await _db.PlaceholderGrowers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.id == id, ct);

        if (entity == null)
        {
            return NotFound();
        }

        var dto = new PlaceholderGrowerDto(
            entity.id,
            entity.grower_name,
            entity.commodity_name,
            entity.created_at,
            entity.updated_at,
            entity.is_active,
            entity.notes
        );

        return Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlaceholderGrowerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlaceholderGrowerDto>> Create(
        [FromBody] PlaceholderGrowerCreateDto input, 
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(input.grower_name))
        {
            return BadRequest("Grower name is required.");
        }

        if (string.IsNullOrWhiteSpace(input.commodity_name))
        {
            return BadRequest("Commodity name is required.");
        }

        var entity = new PlaceholderGrower
        {
            id = Guid.NewGuid(),
            grower_name = input.grower_name.Trim(),
            commodity_name = input.commodity_name.Trim(),
            created_at = DateTime.UtcNow,
            is_active = input.is_active,
            notes = input.notes?.Trim()
        };

        _db.PlaceholderGrowers.Add(entity);
        await _db.SaveChangesAsync(ct);

        var dto = new PlaceholderGrowerDto(
            entity.id,
            entity.grower_name,
            entity.commodity_name,
            entity.created_at,
            entity.updated_at,
            entity.is_active,
            entity.notes
        );

        return CreatedAtAction(nameof(Get), new { id = entity.id, version = "1" }, dto);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PlaceholderGrowerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlaceholderGrowerDto>> Update(
        Guid id, 
        [FromBody] PlaceholderGrowerUpdateDto input, 
        CancellationToken ct)
    {
        var entity = await _db.PlaceholderGrowers
            .FirstOrDefaultAsync(x => x.id == id, ct);

        if (entity == null)
        {
            return NotFound();
        }

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(input.grower_name))
        {
            entity.grower_name = input.grower_name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(input.commodity_name))
        {
            entity.commodity_name = input.commodity_name.Trim();
        }

        if (input.is_active.HasValue)
        {
            entity.is_active = input.is_active.Value;
        }

        if (input.notes != null)
        {
            entity.notes = string.IsNullOrWhiteSpace(input.notes) ? null : input.notes.Trim();
        }

        entity.updated_at = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        var dto = new PlaceholderGrowerDto(
            entity.id,
            entity.grower_name,
            entity.commodity_name,
            entity.created_at,
            entity.updated_at,
            entity.is_active,
            entity.notes
        );

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var entity = await _db.PlaceholderGrowers
            .FirstOrDefaultAsync(x => x.id == id, ct);

        if (entity == null)
        {
            return NotFound();
        }

        _db.PlaceholderGrowers.Remove(entity);
        await _db.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpPatch("{id}/toggle-active")]
    [ProducesResponseType(typeof(PlaceholderGrowerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlaceholderGrowerDto>> ToggleActive(Guid id, CancellationToken ct)
    {
        var entity = await _db.PlaceholderGrowers
            .FirstOrDefaultAsync(x => x.id == id, ct);

        if (entity == null)
        {
            return NotFound();
        }

        entity.is_active = !entity.is_active;
        entity.updated_at = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        var dto = new PlaceholderGrowerDto(
            entity.id,
            entity.grower_name,
            entity.commodity_name,
            entity.created_at,
            entity.updated_at,
            entity.is_active,
            entity.notes
        );

        return Ok(dto);
    }
}
