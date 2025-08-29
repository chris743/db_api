using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Domain;

namespace Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ScoutReportsWithBlockController : ControllerBase
{
    private readonly AppDbContext _db;
    public ScoutReportsWithBlockController(AppDbContext db) => _db = db;

    /// <summary>
    /// Get top 1000 Blocks
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ScoutReportWithBlock>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ScoutReportWithBlock>>> Get(CancellationToken ct)
    {
        var scoutreportswithblock = await _db.ScoutReportsWithBlock
            .AsNoTracking()
            .ToListAsync(ct);

        return Ok(scoutreportswithblock);
    }
}