using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Domain;
using Api.Contracts;
using Microsoft.AspNetCore.Authorization;


namespace Api.Controllers;


/// <summary>Manage harvest plans.</summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize] // Require authentication for all endpoints
public class HarvestPlansController : ControllerBase
{
private readonly AppDbContext _db;
private readonly AuthDbContext _authDb;

public HarvestPlansController(AppDbContext db, AuthDbContext authDb)
{
    _db = db;
    _authDb = authDb;
}


    // GET: /api/v1/HarvestPlans?skip=0&take=100
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HarvestPlanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HarvestPlanDto>>> List([FromQuery] int skip = 0, [FromQuery] int take = 100, CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 500);
        
        // Get harvest plans 
        var harvestPlans = await _db.HarvestPlans.AsNoTracking()
            .OrderByDescending(x => x.date)
            .Skip(skip).Take(take)
            .ToListAsync(ct);

        // Get all unique field representative IDs
        var fieldRepIds = harvestPlans
            .Where(hp => hp.field_representative_id.HasValue)
            .Select(hp => hp.field_representative_id!.Value)
            .Distinct()
            .ToList();

        // Get all unique pool IDs
        var poolIds = harvestPlans
            .Where(hp => hp.pool_id.HasValue)
            .Select(hp => hp.pool_id!.Value)
            .Distinct()
            .ToList();

        // Load all users for field representatives in one query
        var users = new Dictionary<int, UserInfoDto>();
        if (fieldRepIds.Any())
        {
            var usersData = await _authDb.Users
                .Where(u => fieldRepIds.Contains(u.Id))
                .Select(u => new UserInfoDto(
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.IsActive
                ))
                .ToListAsync(ct);
                
            users = usersData.ToDictionary(u => u.id, u => u);
        }

        // Load all pools in one query
        var pools = new Dictionary<int, PoolInfoDto>();
        if (poolIds.Any())
        {
            var poolsData = await _db.Pools
                .Where(p => poolIds.Contains(p.POOLIDX))
                .Select(p => new PoolInfoDto(
                    p.POOLIDX,
                    p.ID,
                    p.DESCR,
                    p.ICCLOSEDFLAG,
                    p.POOLTYPE,
                    p.CMTYIDX,
                    p.VARIETYIDX,
                    p.ICDATEFROM,
                    p.ICDATETHRU,
                    p.DEPTIDX,
                    p.COSTCENTERIDX,
                    p.GARUNIDX,
                    p.ADVRUNIDX,
                    p.source_database
                ))
                .ToListAsync(ct);
                
            pools = poolsData.ToDictionary(p => p.poolidx, p => p);
        }

        // Get block and commodity data separately 
        var harvestPlanResults = new List<HarvestPlanDto>();
        
        foreach (var harvestPlan in harvestPlans)
        {
            // Get block data
            var block = await _db.Blocks
                .Where(b => b.source_database == harvestPlan.grower_block_source_database && b.GABLOCKIDX == harvestPlan.grower_block_id)
                .Select(b => new BlockInfoDto(
                    b.ID,
                    b.NAME,
                    b.BLOCKTYPE,
                    b.GrowerName,
                    b.GrowerID,
                    b.ACRES,
                    b.DISTRICT,
                    b.CROPYEARDESCR,
                    b.LATITUDE,
                    b.LONGITUDE
                ))
                .FirstOrDefaultAsync(ct);

            // Get commodity data
            CommodityInfoDto? commodity = null;
            if (block != null)
            {
                var blockWithCommodity = await _db.Blocks
                    .Where(b => b.source_database == harvestPlan.grower_block_source_database && b.GABLOCKIDX == harvestPlan.grower_block_id)
                    .FirstOrDefaultAsync(ct);
                
                if (blockWithCommodity?.CMTYIDX.HasValue == true)
                {
                    commodity = await _db.Commodities
                        .Where(c => c.source_database == harvestPlan.grower_block_source_database && c.CommodityIDx == blockWithCommodity.CMTYIDX.Value)
                        .Select(c => new CommodityInfoDto(c.InvoiceCommodity, c.Commodity))
                        .FirstOrDefaultAsync(ct);
                }
            }

            // Lookup field representative data from pre-loaded users
            UserInfoDto? fieldRepresentative = null;
            if (harvestPlan.field_representative_id.HasValue && users.ContainsKey(harvestPlan.field_representative_id.Value))
            {
                fieldRepresentative = users[harvestPlan.field_representative_id.Value];
            }

            // Lookup pool data from pre-loaded pools
            PoolInfoDto? pool = null;
            if (harvestPlan.pool_id.HasValue && pools.ContainsKey(harvestPlan.pool_id.Value))
            {
                pool = pools[harvestPlan.pool_id.Value];
            }

            harvestPlanResults.Add(new HarvestPlanDto(
                harvestPlan.id,
                harvestPlan.grower_block_source_database,
                harvestPlan.grower_block_id,
                harvestPlan.placeholder_grower_id,
                harvestPlan.field_representative_id,
                harvestPlan.planned_bins,
                harvestPlan.contractor_id,
                harvestPlan.harvesting_rate,
                harvestPlan.hauler_id,
                harvestPlan.hauling_rate,
                harvestPlan.forklift_contractor_id,
                harvestPlan.forklift_rate,
                harvestPlan.pool_id,
                harvestPlan.notes_general,
                harvestPlan.deliver_to,
                harvestPlan.packed_by,
                harvestPlan.date,
                harvestPlan.bins,
                block,
                commodity,
                fieldRepresentative,
                pool
            ));
        }

        return Ok(harvestPlanResults);
    }
// GET: /api/v1/HarvestPlans/{id}
[HttpGet("{id:guid}")]
[ProducesResponseType(typeof(HarvestPlanDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<HarvestPlanDto>> Get(Guid id, CancellationToken ct)
{
var x = await _db.HarvestPlans.AsNoTracking().FirstOrDefaultAsync(p => p.id == id, ct);
if (x is null) return NotFound();

// Get block data
var block = await _db.Blocks
    .Where(b => b.source_database == x.grower_block_source_database && b.GABLOCKIDX == x.grower_block_id)
    .Select(b => new BlockInfoDto(
        b.ID,
        b.NAME,
        b.BLOCKTYPE,
        b.GrowerName,
        b.GrowerID,
        b.ACRES,
        b.DISTRICT,
        b.CROPYEARDESCR,
        b.LATITUDE,
        b.LONGITUDE
    ))
    .FirstOrDefaultAsync(ct);

// Get commodity data
CommodityInfoDto? commodity = null;
if (block != null)
{
    var blockWithCommodity = await _db.Blocks
        .Where(b => b.source_database == x.grower_block_source_database && b.GABLOCKIDX == x.grower_block_id)
        .FirstOrDefaultAsync(ct);
    
    if (blockWithCommodity?.CMTYIDX.HasValue == true)
    {
        commodity = await _db.Commodities
            .Where(c => c.source_database == x.grower_block_source_database && c.CommodityIDx == blockWithCommodity.CMTYIDX.Value)
            .Select(c => new CommodityInfoDto(c.InvoiceCommodity, c.Commodity))
            .FirstOrDefaultAsync(ct);
    }
}

// Get field representative data
UserInfoDto? fieldRepresentative = null;
if (x.field_representative_id.HasValue)
{
    fieldRepresentative = await _authDb.Users
        .Where(u => u.Id == x.field_representative_id.Value)
        .Select(u => new UserInfoDto(
            u.Id,
            u.Username,
            u.FullName,
            u.Email,
            u.Role,
            u.IsActive
        ))
        .FirstOrDefaultAsync(ct);
}

// Get pool data
PoolInfoDto? pool = null;
if (x.pool_id.HasValue)
{
    pool = await _db.Pools
        .Where(p => p.POOLIDX == x.pool_id.Value)
        .Select(p => new PoolInfoDto(
            p.POOLIDX,
            p.ID,
            p.DESCR,
            p.ICCLOSEDFLAG,
            p.POOLTYPE,
            p.CMTYIDX,
            p.VARIETYIDX,
            p.ICDATEFROM,
            p.ICDATETHRU,
            p.DEPTIDX,
            p.COSTCENTERIDX,
            p.GARUNIDX,
            p.ADVRUNIDX,
            p.source_database
        ))
        .FirstOrDefaultAsync(ct);
}

return new HarvestPlanDto(x.id, x.grower_block_source_database, x.grower_block_id, x.placeholder_grower_id, x.field_representative_id, x.planned_bins, x.contractor_id, x.harvesting_rate, x.hauler_id, x.hauling_rate, x.forklift_contractor_id, x.forklift_rate, x.pool_id, x.notes_general, x.deliver_to, x.packed_by, x.date, x.bins, block, commodity, fieldRepresentative, pool);
}


    // POST: /api/v1/HarvestPlans
    [HttpPost]
    [ProducesResponseType(typeof(HarvestPlanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HarvestPlanDto>> Create(HarvestPlanCreateDto input, CancellationToken ct)
    {
        // Server-side ID assignment
        var entity = new HarvestPlan
        {
            id = Guid.NewGuid(),
            grower_block_source_database = input.grower_block_source_database ?? "",
            grower_block_id = input.grower_block_id ?? 0,
            placeholder_grower_id = input.placeholder_grower_id,
            field_representative_id = input.field_representative_id,
            planned_bins = input.planned_bins,
            contractor_id = input.contractor_id,
            harvesting_rate = input.harvesting_rate,
            hauler_id = input.hauler_id,
            hauling_rate = input.hauling_rate,
            forklift_contractor_id = input.forklift_contractor_id,
            forklift_rate = input.forklift_rate,
            pool_id = input.pool_id,
            notes_general = input.notes_general,
            deliver_to = input.deliver_to,
            packed_by = input.packed_by,
            date = input.date,
            bins = input.bins
        };


        _db.HarvestPlans.Add(entity);
        await _db.SaveChangesAsync(ct);


        // Get field representative data for response
        UserInfoDto? fieldRepresentative = null;
        if (entity.field_representative_id.HasValue)
        {
            fieldRepresentative = await _authDb.Users
                .Where(u => u.Id == entity.field_representative_id.Value)
                .Select(u => new UserInfoDto(
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.IsActive
                ))
                .FirstOrDefaultAsync(ct);
        }

        // Get pool data for response
        PoolInfoDto? pool = null;
        if (entity.pool_id.HasValue)
        {
            pool = await _db.Pools
                .Where(p => p.POOLIDX == entity.pool_id.Value)
                .Select(p => new PoolInfoDto(
                    p.POOLIDX,
                    p.ID,
                    p.DESCR,
                    p.ICCLOSEDFLAG,
                    p.POOLTYPE,
                    p.CMTYIDX,
                    p.VARIETYIDX,
                    p.ICDATEFROM,
                    p.ICDATETHRU,
                    p.DEPTIDX,
                    p.COSTCENTERIDX,
                    p.GARUNIDX,
                    p.ADVRUNIDX,
                    p.source_database
                ))
                .FirstOrDefaultAsync(ct);
        }

        var dto = new HarvestPlanDto(entity.id, entity.grower_block_source_database, entity.grower_block_id, entity.placeholder_grower_id, entity.field_representative_id, entity.planned_bins, entity.contractor_id, entity.harvesting_rate, entity.hauler_id, entity.hauling_rate, entity.forklift_contractor_id, entity.forklift_rate, entity.pool_id, entity.notes_general, entity.deliver_to, entity.packed_by, entity.date, entity.bins, null, null, fieldRepresentative, pool);
        return CreatedAtAction(nameof(Get), new { id = entity.id, version = "1" }, dto);
    }
// PUT: /api/v1/HarvestPlans/{id}
[HttpPut("{id:guid}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Update(Guid id, HarvestPlanUpdateDto input, CancellationToken ct)
{
var x = await _db.HarvestPlans.FirstOrDefaultAsync(p => p.id == id, ct);
if (x is null) return NotFound();


if (input.grower_block_source_database != null) x.grower_block_source_database = input.grower_block_source_database;
if (input.grower_block_id.HasValue) x.grower_block_id = input.grower_block_id.Value;
x.placeholder_grower_id = input.placeholder_grower_id ?? x.placeholder_grower_id;
x.field_representative_id = input.field_representative_id ?? x.field_representative_id;
x.planned_bins = input.planned_bins ?? x.planned_bins;
x.contractor_id = input.contractor_id ?? x.contractor_id;
x.harvesting_rate = input.harvesting_rate ?? x.harvesting_rate;
x.hauler_id = input.hauler_id ?? x.hauler_id;
x.hauling_rate = input.hauling_rate ?? x.hauling_rate;
x.forklift_contractor_id = input.forklift_contractor_id ?? x.forklift_contractor_id;
x.forklift_rate = input.forklift_rate ?? x.forklift_rate;
x.pool_id = input.pool_id ?? x.pool_id;
x.notes_general = input.notes_general ?? x.notes_general;
x.deliver_to = input.deliver_to ?? x.deliver_to;
x.packed_by = input.packed_by ?? x.packed_by;
x.date = input.date ?? x.date;
x.bins = input.bins ?? x.bins;


await _db.SaveChangesAsync(ct);
return NoContent();
}


// DELETE: /api/v1/HarvestPlans/{id}
[HttpDelete("{id:guid}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
{
var x = await _db.HarvestPlans.FirstOrDefaultAsync(p => p.id == id, ct);
if (x is null) return NotFound();
_db.HarvestPlans.Remove(x);
await _db.SaveChangesAsync(ct);
return NoContent();
}
}