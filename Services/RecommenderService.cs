using FinFlowAPI.Services.User;

public class RecommenderService
{
    private readonly IUserService _user;
    private readonly IStockService _stock;
    private readonly IRecommendationService _rec;
    private readonly StockRecommendationService _engine;
    private readonly GeminiService _gemini;
    private readonly StockCacheRefreshService _refreshService;
    private readonly ILogger<RecommenderService> _logger;

    public RecommenderService(
        StockCacheRefreshService refreshService,
        ILogger<RecommenderService> logger,
        IUserService user,
        IStockService stock,
        IRecommendationService rec,
        StockRecommendationService engine,
        GeminiService gemini)
    {
        _refreshService = refreshService;
        _logger = logger;
        _user = user;
        _stock = stock;
        _rec = rec;
        _engine = engine;
        _gemini = gemini;
    }

    public async Task<List<RecommendationDTO>> GenerateAsync(
        string userId, bool refresh = false)
    {
        if (!refresh)
        {
            var cached = await _rec.GetCachedAsync(userId, maxAgeHours: 6);
            if (cached != null && cached.Count > 0)
                return cached;
        }

        var user = await _user.GetUserProfileAsync(Convert.ToInt32(userId));
        if (user == null)
            return new List<RecommendationDTO>();

        var preferredSectors = user.PreferredSectors;
        var stocks = await _stock.GetStocksForScoringAsync(userId);
        if (stocks == null || stocks.Count == 0)
        {
            _logger.LogWarning("Stock cache empty for user {UserId}. Attempting auto-refresh.", userId);

            try
            {
                int refreshed = await _refreshService.RefreshAsync();
                _logger.LogInformation("Auto-refreshed {Count} stocks.", refreshed);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "NEPSE API unavailable. Seeding sample data.");
                await _refreshService.SeedSampleDataAsync();
            }

            stocks = await _stock.GetStocksForScoringAsync(userId);

            if (stocks == null || stocks.Count == 0)
                return new List<RecommendationDTO>(); 
        }

        var picks = _engine.Score(user, stocks);

        if (picks.Count == 0)
            return new List<RecommendationDTO>();

        Dictionary<string, string> explanations = new Dictionary<string, string>();
        try
        {
            explanations = await _gemini.GetExplanationsAsync(user, picks);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Gemini API call failed. Proceeding without AI explanations.");
        }

        await _rec.DeleteAllForUserAsync(userId);

        foreach (var (stock, score, breakdown) in picks)
        {
            explanations.TryGetValue(stock.Symbol, out string explanation);
            await _rec.SaveAsync(
                userId, stock, score, breakdown,
                explanation ?? "Analysis not available.");
        }

        return await _rec.GetCachedAsync(userId, maxAgeHours: 1);
    }
}