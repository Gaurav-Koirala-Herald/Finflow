public class StockDTO
    {
        public string Symbol { get; set; } = string.Empty;
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