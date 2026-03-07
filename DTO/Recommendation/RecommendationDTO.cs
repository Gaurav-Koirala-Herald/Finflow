public class RecommendationRequestDTO
{
    public string UserId { get; set; } = string.Empty;
    public bool RefreshExplanation { get; set; } = false;
}
public class RecommendationDTO
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal TrendScore { get; set; }
    public decimal FundamentalsScore { get; set; }
    public decimal SectorScore { get; set; }
    public decimal RiskScore { get; set; }
    public decimal PopularityScore { get; set; }
    public string AiExplanation { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    // Flattened stock fields returned by sp_GetRecommendations
    public string SecurityName { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal Ltp { get; set; }
    public decimal PointChange { get; set; }
    public decimal PercentageChange { get; set; }
    public decimal PriceChange30d { get; set; }
    public decimal PeRatio { get; set; }
    public long MarketCap { get; set; }
    public long Volume { get; set; }
    public decimal High52week { get; set; }
    public decimal Low52week { get; set; }
    public decimal Eps { get; set; }
    public decimal BookValue { get; set; }
}