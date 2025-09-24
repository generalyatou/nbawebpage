using Microsoft.AspNetCore.Mvc;
using NBA.Business.Services;
using NBA.common.Entities;
using NBA.data.Services;

namespace NBA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TeamSummariesController : ControllerBase
{
    private readonly INbaDetailsService _nbaDetailsService;
    private readonly INbaDetailsMapping _nbaDetailsMapping;

    public TeamSummariesController(
        INbaDetailsService nbaDetailsService,
        INbaDetailsMapping nbaDetailsMapping)
    {
        _nbaDetailsService = nbaDetailsService;
        _nbaDetailsMapping = nbaDetailsMapping;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamSummaries>>> Get(
        [FromQuery] int? teamId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken ct)
    {
        var facts = await _nbaDetailsService.RetrieveNBADetailsAsync(startDate, endDate, teamId, ct);
        var summaries = _nbaDetailsMapping.BuildSummariesMapping(facts);
        return Ok(summaries);
    }
}
