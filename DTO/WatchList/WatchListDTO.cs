public class WatchlistItemDTO
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal? AlertPrice { get; set; }
    public DateTime AddedAt { get; set; }
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

public class AddWatchlistDTO
{
    public string Symbol { get; set; } = string.Empty;
    public decimal? AlertPrice { get; set; }
}
