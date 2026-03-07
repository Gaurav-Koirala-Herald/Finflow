using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/recommendations")]
public class RecommendationsController : ControllerBase
{
    private readonly RecommenderService _orchestrator;
    private readonly IRecommendationService _rec;

    public RecommendationsController(
        RecommenderService orchestrator,
        IRecommendationService rec)
    {
        _orchestrator = orchestrator;
        _rec = rec;
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
}