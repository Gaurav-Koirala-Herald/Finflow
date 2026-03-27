using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/recommendations")]
public class RecommendationsController : ControllerBase
{
    private readonly RecommenderService _orchestrator;
    private readonly IRecommendationService _rec;

    private readonly StockCacheRefreshService _refreshService;

    public RecommendationsController(
        RecommenderService orchestrator,
        IRecommendationService rec,
        StockCacheRefreshService refreshService)
    {
        _orchestrator = orchestrator;
        _rec = rec;
        _refreshService = refreshService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCached(string userId)
    {
        var dbResp = await _rec.GetCachedAsync(userId);
        return Ok(dbResp);
    }

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] RecommendationRequestDTO dto)
    {
        var dbResp = await _orchestrator.GenerateAsync(dto.UserId, dto.RefreshExplanation);
        return Ok(dbResp);
    }
    [HttpPost("refresh-cache")]
    public async Task<IActionResult> RefreshCache()
    {
        var (count, source) = await _refreshService.SmartRefreshAsync();
        return Ok(new { success = true, message = $"{count} stocks loaded from {source}." });
    }

    [HttpPost("seed-cache")]
    public async Task<IActionResult> SeedCache()
    {
        await _refreshService.SeedRealDataAsync();
        return Ok(new { success = true, message = "Real NEPSE stock data seeded (10 stocks)." });
    }
}