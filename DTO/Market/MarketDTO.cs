public class MarketSummaryDTO
{
    public decimal NepseIndex { get; set; }
    public decimal NepseChangePercent { get; set; }
    public decimal TotalTurnover { get; set; }
    public int AdvancingStocks { get; set; }
    public int DecliningStocks { get; set; }
    public int UnchangedStocks { get; set; }
    public bool IsMarketOpen { get; set; }
    public double TotalTransactions {get;set;}
    public double TotalTradedShares {get;set;}
    public DateTime LastUpdated { get; set; }
}