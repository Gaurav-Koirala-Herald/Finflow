 public class StockCacheRefreshService
    {
        private readonly IStockService _stock;
        private readonly NepseApiService  _nepse;

        public StockCacheRefreshService(IStockService stock, NepseApiService nepse)
        {
            _stock = stock;
            _nepse     = nepse;
        }

        public async Task RefreshAsync()
        {
            var liveMarket = await _nepse.GetLiveMarketAsync();
            var securities = await _nepse.GetSecurityListAsync();
            var sectorMap  = securities.ToDictionary(s => s.Symbol, s => s.Sector);

            foreach (var stock in liveMarket)
            {
                decimal change30d = 0;
                try
                {
                    var history = await _nepse.GetDailyPriceGraphAsync(stock.Symbol);
                    if (history.Count >= 2)
                    {
                        decimal oldest = history.OrderBy(h => h.Date).First().ClosePrice;
                        if (oldest > 0)
                            change30d = ((stock.Ltp - oldest) / oldest) * 100;
                    }
                }
                catch { /* skip if unavailable */ }

                var stockCache = new StockCache
                {
                    Symbol           = stock.Symbol,
                    SecurityName     = stock.SecurityName,
                    Sector           = sectorMap.GetValueOrDefault(stock.Symbol, "Other"),
                    CurrentPrice     = stock.Ltp,
                    Ltp              = stock.Ltp,
                    PointChange      = stock.PointChange,
                    PercentageChange = stock.PercentageChange,
                    PriceChange30d   = change30d,
                    Volume           = stock.TotalTradeQuantity,
                    LastUpdated      = DateTime.UtcNow
                };

                await _stock.UpsertStockAsync(stockCache);
            }
        }
    }