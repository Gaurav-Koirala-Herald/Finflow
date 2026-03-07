using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/market")]
[Authorize]
public class MarketController : ControllerBase
{
    private readonly NepseApiService _nepse;

    public MarketController(NepseApiService nepse)
    {
        _nepse = nepse;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var indexTask = _nepse.GetNepseIndexAsync();
        var statusTask = _nepse.GetNepseStatusAsync();
        var summaryTask = _nepse.GetSummaryAsync();

        await Task.WhenAll(indexTask, statusTask, summaryTask);

        var index = indexTask.Result;
        var status = statusTask.Result;
        var summary = summaryTask.Result;

        index.TryGetValue("NEPSE", out var nepseEntry);

        var dto = new MarketSummaryDTO
        {
            NepseIndex = nepseEntry?.CurrentValue ?? 0,
            NepseChangePercent = nepseEntry?.PerChange ?? 0,
            TotalTurnover = summary.TotalTurnover,
            AdvancingStocks = summary.AdvancingCount,
            DecliningStocks = summary.DecliningCount,
            UnchangedStocks = summary.UnchangedCount,
            IsMarketOpen = status.IsOpen == "OPEN",
            LastUpdated = DateTime.UtcNow
        };

        return Ok(dto);
    }

    [HttpGet("gainers")]
    public async Task<IActionResult> GetTopGainers()
    {
        var dbResp = await _nepse.GetTopGainersAsync();
        return Ok(dbResp);
    }

    [HttpGet("losers")]
    public async Task<IActionResult> GetTopLosersAsync()
    {
        var dbResp = await _nepse.GetTopLosersAsync();
        return Ok(dbResp);
    }
}