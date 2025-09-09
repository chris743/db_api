using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;                 // your AppDbContext namespace
using Api.Domain;              // ProcessProductionRun entity namespace
using Api.Contracts;            // DTOs namespace

namespace Api.Controllers;

/// <summary>Manage production runs.</summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ProcessProductionRunsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProcessProductionRunsController(AppDbContext db) => _db = db;

    // GET: /api/v1/ProcessProductionRuns?skip=0&take=100&sourceDatabase=Famous&gaBlockIdx=12345
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductionRunDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductionRunDto>>> List(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100,
        [FromQuery] string? source_Database = null,
        [FromQuery] int? gaBlockIdx = null,
        CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 500);

        var q = _db.ProcessProductionRuns.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(source_Database))
            q = q.Where(r => r.source_database == source_Database);

        if (gaBlockIdx.HasValue)
            q = q.Where(r => r.GABLOCKIDX == gaBlockIdx.Value);

        var rows = await q
            .Include(r => r.Block)
            .OrderByDescending(r => r.run_date)
            .Skip(skip).Take(take)
            .Select(r => new ProductionRunDto(
                r.id,
                r.source_database,
                r.GABLOCKIDX,
                r.bins,
                r.run_date,
                r.pick_date,
                r.location,
                r.pool,
                r.notes,
                r.row_order,
                r.run_status,
                r.batch_id,
                r.time_started,
                r.time_completed,
                r.Block != null ? new BlockInfoDto(
                    r.Block.ID,
                    r.Block.NAME,
                    r.Block.BLOCKTYPE,
                    r.Block.GrowerName,
                    r.Block
                    .GrowerID,
                    r.Block.ACRES,
                    r.Block.DISTRICT,
                    r.Block.CROPYEARDESCR,
                    r.Block.LATITUDE,
                    r.Block.LONGITUDE
                ) : null,
                r.Block != null && r.Block.CMTYIDX.HasValue ? 
                    _db.Commodities
                        .Where(c => c.source_database == r.source_database && c.CommodityIDx == r.Block.CMTYIDX.Value)
                        .Select(c => new CommodityInfoDto(c.InvoiceCommodity, c.Commodity))
                        .FirstOrDefault() : null
            ))
            .ToListAsync(ct);

        return Ok(rows);
    }

    // GET: /api/v1/ProcessProductionRuns/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductionRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductionRunDto>> Get(Guid id, CancellationToken ct)
    {
        var r = await _db.ProcessProductionRuns
            .Include(x => x.Block)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.id == id, ct);
        if (r is null) return NotFound();

        // Get commodity data if block has commodity index
        CommodityInfoDto? commodity = null;
        if (r.Block != null && r.Block.CMTYIDX.HasValue)
        {
            var commodityData = await _db.Commodities
                .Where(c => c.source_database == r.source_database && c.CommodityIDx == r.Block.CMTYIDX.Value)
                .Select(c => new CommodityInfoDto(c.InvoiceCommodity, c.Commodity))
                .FirstOrDefaultAsync(ct);
            commodity = commodityData;
        }

        return new ProductionRunDto(
            r.id, r.source_database, r.GABLOCKIDX, r.bins, r.run_date, r.pick_date,
            r.location, r.pool, r.notes, r.row_order, r.run_status, r.batch_id,
            r.time_started, r.time_completed,
            r.Block != null ? new BlockInfoDto(
                r.Block.ID,
                r.Block.NAME,
                r.Block.BLOCKTYPE,
                r.Block.GrowerName,
                r.Block.GrowerID,
                r.Block.ACRES,
                r.Block.DISTRICT,
                r.Block.CROPYEARDESCR,
                r.Block.LATITUDE,
                r.Block.LONGITUDE
            ) : null,
            commodity
        );
    }

    // POST: /api/v1/ProcessProductionRuns
    [HttpPost]
    [ProducesResponseType(typeof(ProductionRunDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductionRunDto>> Create(ProductionRunCreateDto input, CancellationToken ct)
    {
        // Basic validation (optional)
        if (string.IsNullOrWhiteSpace(input.source_database))
            return BadRequest("source_database is required.");
        if (input.GABLOCKIDX <= 0)
            return BadRequest("GABLOCKIDX must be > 0.");

        var entity = new ProcessProductionRun
        {
            id = Guid.NewGuid(),
            source_database = input.source_database,
            GABLOCKIDX = input.GABLOCKIDX,
            bins = input.bins ?? 0,
            run_date = input.run_date,
            pick_date = input.pick_date,
            location = input.location,
            pool = input.pool,
            notes = input.notes,
            row_order = input.row_order,
            run_status = input.run_status,
            batch_id = input.batch_id,
            time_started = input.time_started,
            time_completed = input.time_completed
        };

        _db.ProcessProductionRuns.Add(entity);
        await _db.SaveChangesAsync(ct);

        var dto = new ProductionRunDto(
            entity.id, entity.source_database, entity.GABLOCKIDX, entity.bins,
            entity.run_date, entity.pick_date, entity.location, entity.pool, entity.notes,
            entity.row_order, entity.run_status, entity.batch_id, entity.time_started, entity.time_completed,
            null, // block - will be null for new entities
            null  // commodity - will be null for new entities
        );

        return CreatedAtAction(nameof(Get), new { id = entity.id, version = "1" }, dto);
    }

    // PUT: /api/v1/ProcessProductionRuns/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, ProductionRunUpdateDto input, CancellationToken ct)
    {
        var r = await _db.ProcessProductionRuns.FirstOrDefaultAsync(x => x.id == id, ct);
        if (r is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(input.source_database)) r.source_database = input.source_database!;
        if (input.GABLOCKIDX.HasValue) r.GABLOCKIDX = input.GABLOCKIDX.Value;

        if (input.bins.HasValue) r.bins = input.bins.Value;
        r.run_date = input.run_date ?? r.run_date;
        r.pick_date = input.pick_date ?? r.pick_date;
        r.location = input.location ?? r.location;
        r.pool = input.pool ?? r.pool;
        r.notes = input.notes ?? r.notes;
        r.row_order = input.row_order ?? r.row_order;
        r.run_status = input.run_status ?? r.run_status;
        r.batch_id = input.batch_id ?? r.batch_id;
        r.time_started = input.time_started ?? r.time_started;
        r.time_completed = input.time_completed ?? r.time_completed;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // DELETE: /api/v1/ProcessProductionRuns/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var r = await _db.ProcessProductionRuns.FirstOrDefaultAsync(x => x.id == id, ct);
        if (r is null) return NotFound();

        _db.ProcessProductionRuns.Remove(r);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
