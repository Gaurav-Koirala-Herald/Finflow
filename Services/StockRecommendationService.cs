using FinFlowAPI.DTO;

public class StockRecommendationService
{
    public List<(StockCache Stock, decimal Score, ScoreBreakdown Breakdown)> Score(
        UserDto user, List<StockCache> stocks)
    {
        var preferredSectors = new HashSet<string>(
            user.PreferredSectors, StringComparer.OrdinalIgnoreCase);

        decimal maxVolume = stocks.Max(s => (decimal)s.Volume);
        decimal minVolume = stocks.Min(s => (decimal)s.Volume);
        decimal maxChange = stocks.Max(s => s.PriceChange30d);
        decimal minChange = stocks.Min(s => s.PriceChange30d);

        var results = new List<(StockCache, decimal, ScoreBreakdown)>();

        foreach (var stock in stocks)
        {
            if (stock.CurrentPrice <= 0) continue;

            var breakdown = new ScoreBreakdown
            {
                TrendScore = Normalize(stock.PriceChange30d, minChange, maxChange),
                FundamentalsScore = stock.PeRatio > 0
                                        ? Normalize(1m / stock.PeRatio, 0m, 0.15m)
                                        : 0.5m,
                SectorScore = preferredSectors.Contains(stock.Sector) ? 1.0m : 0.3m,
                RiskScore = MatchRisk(user.RiskLevel, stock),
                PopularityScore = Normalize((decimal)stock.Volume, minVolume, maxVolume)
            };

            decimal score = Clamp(
                (0.30m * breakdown.TrendScore) +
                (0.25m * breakdown.FundamentalsScore) +
                (0.20m * breakdown.SectorScore) +
                (0.15m * breakdown.RiskScore) +
                (0.10m * breakdown.PopularityScore));

            results.Add((stock, score, breakdown));
        }

        return results
            .OrderByDescending(r => r.Item2)
            .Take(5)
            .ToList();
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