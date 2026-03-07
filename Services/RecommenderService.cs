using FinFlowAPI.Services.User;

public class RecommenderService
{
    private readonly IUserService _user;
    private readonly IStockService _stock;
    private readonly IRecommendationService _rec;
    private readonly StockRecommendationService _engine;
    private readonly GeminiService _gemini;

    public RecommenderService(
        IUserService user,
        IStockService stock,
        IRecommendationService rec,
        StockRecommendationService engine,
        GeminiService gemini)
    {
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
            throw new Exception("User not found");

        var stocks = await _stock.GetStocksForScoringAsync(userId);
        if (stocks == null || stocks.Count == 0)
            throw new Exception("No stock data available. Run the cache refresh job first.");

        var picks = _engine.Score(user, stocks);

        var explanations = await _gemini.GetExplanationsAsync(user, picks);

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