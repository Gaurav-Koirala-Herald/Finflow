using FinFlowAPI.DTO;

public class StockRecommendationService
{
    public List<(StockCache Stock, decimal Score, ScoreBreakdown Breakdown)> Score(
    UserDto user, List<StockCache> stocks)
{
    if (stocks == null || stocks.Count == 0)
        return new();

    var rnd = new Random();

    var preferredSectors = new HashSet<string>(
        user.PreferredSectors ?? new List<string>(),
        StringComparer.OrdinalIgnoreCase);

    if (preferredSectors.Any())
    {
        var filtered = stocks
            .Where(s => preferredSectors.Contains(s.Sector))
            .ToList();

        if (filtered.Count > 0)
            stocks = filtered; 
    }

    bool useFinancials = user.TotalIncome > 0 && user.TotalExpense > 0;

    decimal disposableIncome = user.TotalIncome - user.TotalExpense;
    decimal investableAmount = disposableIncome > 0 ? disposableIncome * 0.3m : 0;

    if (useFinancials && investableAmount > 0)
    {
        var filtered = stocks
            .Where(s => s.CurrentPrice <= investableAmount * 2) 
            .ToList();

        if (filtered.Count > 0)
            stocks = filtered;
    }

   
    decimal maxVolume = stocks.Max(s => (decimal)s.Volume);
    decimal minVolume = stocks.Min(s => (decimal)s.Volume);
    decimal maxChange = stocks.Max(s => s.PriceChange30d);
    decimal minChange = stocks.Min(s => s.PriceChange30d);


    decimal trendW, fundW, sectorW, riskW, volumeW, affordabilityW;

    switch (user.RiskLevel)
    {
        case "Low":
            trendW = 0.15m;
            fundW = 0.30m;
            sectorW = 0.20m;
            riskW = 0.20m;
            volumeW = 0.05m;
            affordabilityW = 0.10m;
            break;

        case "High":
            trendW = 0.40m;
            fundW = 0.10m;
            sectorW = 0.15m;
            riskW = 0.20m;
            volumeW = 0.10m;
            affordabilityW = 0.05m;
            break;

        default: // Medium
            trendW = 0.30m;
            fundW = 0.20m;
            sectorW = 0.20m;
            riskW = 0.15m;
            volumeW = 0.10m;
            affordabilityW = 0.05m;
            break;
    }

    var results = new List<(StockCache, decimal, ScoreBreakdown)>();

    foreach (var stock in stocks)
    {
        if (stock.CurrentPrice <= 0) continue;

        decimal affordabilityScore = 0.5m;

        if (useFinancials && investableAmount > 0)
        {
            affordabilityScore = stock.CurrentPrice <= investableAmount
                ? 1.0m
                : Clamp(investableAmount / stock.CurrentPrice);
        }

        decimal volatility = Math.Abs(stock.PercentageChange);

        decimal riskScore = user.RiskLevel switch
        {
            "Low" => volatility < 2 ? 1.0m : 0.3m,
            "Medium" => volatility < 5 ? 1.0m : 0.6m,
            "High" => volatility >= 5 ? 1.0m : 0.5m,
            _ => 0.5m
        };

        var breakdown = new ScoreBreakdown
        {
            TrendScore = Normalize(stock.PriceChange30d, minChange, maxChange),
            FundamentalsScore = stock.PeRatio > 0
                ? Normalize(1m / stock.PeRatio, 0m, 0.15m)
                : 0.5m,
            SectorScore = preferredSectors.Contains(stock.Sector) ? 1.0m : 0.5m,
            RiskScore = riskScore,
            PopularityScore = Normalize((decimal)stock.Volume, minVolume, maxVolume),
            AffordabilityScore = affordabilityScore
        };

        decimal score = Clamp(
            (trendW * breakdown.TrendScore) +
            (fundW * breakdown.FundamentalsScore) +
            (sectorW * breakdown.SectorScore) +
            (riskW * breakdown.RiskScore) +
            (volumeW * breakdown.PopularityScore) +
            (affordabilityW * breakdown.AffordabilityScore)
        );

        score += (decimal)rnd.NextDouble() * 0.03m;

        results.Add((stock, score, breakdown));
    }

    var diversified = results
        .OrderByDescending(r => r.Item2)
        .GroupBy(r => r.Item1.Sector)
        .SelectMany(g => g.Take(2)) 
        .OrderByDescending(r => r.Item2)
        .Take(5)
        .ToList();

    return diversified;
}

    private static decimal Normalize(decimal value, decimal min, decimal max)
    {
        if (max == min) return 0.5m;
        return Clamp((value - min) / (max - min));
    }

    private static decimal Clamp(decimal value) =>
        Math.Max(0m, Math.Min(1m, value));

    private static decimal MatchRisk(string riskLevel, StockCache stock)
    {
        return riskLevel switch
        {
            "Low" when stock.MarketCap > 5_000_000_000L => 1.0m,
            "Low" when stock.MarketCap > 2_000_000_000L => 0.7m,
            "Medium" when stock.MarketCap > 2_000_000_000L => 1.0m,
            "High" when stock.MarketCap < 2_000_000_000L => 1.0m,
            "High" when stock.MarketCap < 5_000_000_000L => 0.7m,
            _ => 0.4m
        };
    }
}