using System.Collections.Concurrent;

public class StockCacheRefreshService
{
    private readonly IStockService _stock;
    private readonly NepseApiService _nepse;
    private readonly ILogger<StockCacheRefreshService> _logger;

    public StockCacheRefreshService(
        IStockService stock,
        NepseApiService nepse,
        ILogger<StockCacheRefreshService> logger)
    {
        _stock = stock;
        _nepse = nepse;
        _logger = logger;
    }

    public async Task<int> RefreshAsync()
    {
        _logger.LogInformation("Starting NEPSE stock cache refresh...");

        List<NepseStockPrice> liveMarket;
        List<NepseSecurity> securities;

        try
        {
            // 🔹 Fetch both in parallel
            var liveTask = _nepse.GetLiveMarketAsync();
            var secTask = _nepse.GetSecurityListAsync();

            await Task.WhenAll(liveTask, secTask);

            liveMarket = liveTask.Result;
            securities = secTask.Result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch NEPSE data.");
            throw;
        }

        if (liveMarket == null || liveMarket.Count == 0)
            throw new Exception("No live market data.");

        var sectorMap = securities
            .Where(s => !string.IsNullOrWhiteSpace(s.Symbol))
            .GroupBy(s => s.Symbol.Trim().ToUpper())
            .ToDictionary(
                g => g.Key,
                g => NormalizeSector(g.First().SectorName)
            );

        var priceChange30dMap = new ConcurrentDictionary<string, decimal>();

        var semaphore = new SemaphoreSlim(5);

        var historyTasks = liveMarket
            .Where(s => !string.IsNullOrWhiteSpace(s.Symbol))
            .Select(async stock =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var history = await _nepse.GetDailyPriceGraphAsync(stock.Symbol);

                    if (history != null && history.Count >= 2)
                    {
                        var ordered = history.OrderBy(h => h.Date).ToList();
                        decimal oldPrice = ordered.First().ClosePrice;
                        decimal newPrice = ordered.Last().ClosePrice;

                        if (oldPrice > 0)
                        {
                            decimal change = ((newPrice - oldPrice) / oldPrice) * 100m;
                            priceChange30dMap[stock.Symbol] = change;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "History fetch failed for {Symbol}", stock.Symbol);
                    priceChange30dMap[stock.Symbol] = 0m;
                }
                finally
                {
                    semaphore.Release();
                }
            });

        await Task.WhenAll(historyTasks);
        var stockCacheList = liveMarket.Select(stock =>
        {
            return new StockCache
            {
                Symbol = stock.Symbol,
                SecurityName = stock.SecurityName,
                Sector = sectorMap.GetValueOrDefault(stock.Symbol, "Other"),
                CurrentPrice = stock.Ltp,
                Ltp = stock.Ltp,
                PointChange = stock.PointChange,
                PercentageChange = stock.PercentageChange,
                PriceChange30d = priceChange30dMap.GetValueOrDefault(stock.Symbol, 0m),
                Volume = stock.TotalTradeQuantity,
                High52week = stock.HighPrice,
                Low52week = stock.LowPrice,
                PeRatio = 0,
                Eps = 0,
                BookValue = 0,
                LastUpdated = DateTime.UtcNow
            };
        }).ToList();

    
        int affected = await _stock.BulkUpsertAsync(stockCacheList);

        _logger.LogInformation("Stock cache refresh complete. {Count} stocks updated.", affected);

        return affected;
    }
    public async Task SeedRealDataAsync()
    {
        var stocks = new List<StockCache>
            {
                new() { Symbol = "NABIL",  SecurityName = "Nabil Bank Ltd.",                Sector = "Banking",          CurrentPrice = 1250.0m, Ltp = 1250.0m, PointChange =  15.0m, PercentageChange =  1.22m, PriceChange30d =  6.5m,  PeRatio = 18.50m, MarketCap = 85000000000, Volume = 320000, High52week = 1450m, Low52week =  950m, Eps = 67.50m, BookValue = 310m, LastUpdated = DateTime.UtcNow },
                new() { Symbol = "NICA",   SecurityName = "NIC Asia Bank Ltd.",              Sector = "Banking",          CurrentPrice =  512.3m, Ltp =  512.3m, PointChange =  18.4m, PercentageChange =  3.73m, PriceChange30d =  8.2m,  PeRatio = 14.20m, MarketCap = 42000000000, Volume = 245000, High52week =  580m, Low52week =  390m, Eps = 36.10m, BookValue = 185m, LastUpdated = DateTime.UtcNow },
                new() { Symbol = "GBIME",  SecurityName = "Global IME Bank Ltd.",            Sector = "Banking",          CurrentPrice =  376.8m, Ltp =  376.8m, PointChange =   9.8m, PercentageChange =  2.67m, PriceChange30d =  6.7m,  PeRatio = 16.30m, MarketCap = 56000000000, Volume = 421000, High52week =  420m, Low52week =  285m, Eps = 23.10m, BookValue = 155m, LastUpdated = DateTime.UtcNow },
                new() { Symbol = "SANIMA", SecurityName = "Sanima Bank Ltd.",                Sector = "Banking",          CurrentPrice =  298.5m, Ltp =  298.5m, PointChange =   5.5m, PercentageChange =  1.88m, PriceChange30d =  4.1m,  PeRatio = 13.80m, MarketCap = 33000000000, Volume = 198000, High52week =  340m, Low52week =  240m, Eps = 21.60m, BookValue = 140m, LastUpdated = DateTime.UtcNow },
                new() { Symbol = "UPPER",  SecurityName = "Upper Tamakoshi Hydropower Ltd.", Sector = "Hydropower",       CurrentPrice =  278.5m, Ltp =  278.5m, PointChange =   6.2m, PercentageChange =  2.28m, PriceChange30d =  5.4m,  PeRatio = 18.70m, MarketCap = 38000000000, Volume = 189000, High52week =  320m, Low52week =  210m, Eps = 14.90m, BookValue =  98m, LastUpdated = DateTime.UtcNow },
                new() { Symbol = "SHIVM",  SecurityName = "Shiva Shree Hydropower Ltd.",     Sector = "Hydropower",       CurrentPrice =  215.0m, Ltp =  215.0m, PointChange =  -4.5m, PercentageChange = -2.05m, PriceChange30d = -3.1m,  PeRatio = 22.10m, MarketCap =  4200000000, Volume =  98000, High52week =  265m, Low52week =  180m, Eps =  9.72m, BookValue =  72m, LastUpdated = DateTime.UtcNow },
                new() { Symbol = "NIFRA",  SecurityName = "Nepal Infrastructure Bank",       Sector = "Development Bank", CurrentPrice =   98.7m, Ltp =   98.7m, PointChange =   3.1m, PercentageChange =  3.24m, PriceChange30d = 11.3m,  PeRatio = 11.40m, MarketCap =  8500000000, Volume = 312000, High52week =  115m, Low52week =   72m, Eps =  8.66m, BookValue =  61m, LastUpdated = DateTime.UtcNow },
                new() { Symbol = "CHCL",   SecurityName = "Chilime Hydropower Co. Ltd.",     Sector = "Hydropower",       CurrentPrice =  540.0m, Ltp =  540.0m, PointChange =  10.0m, PercentageChange =  1.89m, PriceChange30d =  7.3m,  PeRatio = 20.50m, MarketCap = 22000000000, Volume = 145000, High52week =  620m, Low52week =  430m, Eps = 26.30m, BookValue = 165m, LastUpdated = DateTime.UtcNow },
                new() { Symbol = "NLG",    SecurityName = "Nepal Life Insurance Co. Ltd.",   Sector = "Insurance",        CurrentPrice = 1820.0m, Ltp = 1820.0m, PointChange =  25.0m, PercentageChange =  1.39m, PriceChange30d =  9.1m,  PeRatio = 24.10m, MarketCap = 28000000000, Volume =  62000, High52week = 2100m, Low52week = 1450m, Eps = 75.50m, BookValue = 420m, LastUpdated = DateTime.UtcNow },
                new() { Symbol = "SICL",   SecurityName = "Siddartha Insurance Ltd.",        Sector = "Insurance",        CurrentPrice =  765.0m, Ltp =  765.0m, PointChange =  12.0m, PercentageChange =  1.59m, PriceChange30d =  5.8m,  PeRatio = 19.30m, MarketCap =  9800000000, Volume =  88000, High52week =  890m, Low52week =  610m, Eps = 39.60m, BookValue = 235m, LastUpdated = DateTime.UtcNow },
            };

        foreach (var stock in stocks)
            await _stock.UpsertStockAsync(stock);

        _logger.LogInformation("Seeded {Count} real NEPSE stocks into cache.", stocks.Count);
    }
    public async Task<(int Count, string Source)> SmartRefreshAsync()
    {
        try
        {
            int count = await RefreshAsync();
            return (count, "NEPSE API");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "NEPSE API unavailable. Falling back to seed data.");
            await SeedRealDataAsync();
            return (10, "Seed Data");
        }
    }

    public async Task SeedSampleDataAsync()
    {
        var sampleStocks = new List<StockCache>
            {
                new StockCache { Symbol = "NABIL",  SecurityName = "Nabil Bank Ltd.",               Sector = "Banking",          CurrentPrice = 1250.0m, Ltp = 1250.0m, PointChange = 15.0m,  PercentageChange = 1.22m,  PriceChange30d = 6.5m,  PeRatio = 18.5m, MarketCap = 85000000000, Volume = 320000, High52week = 1450m, Low52week = 950m,  Eps = 67.5m,  BookValue = 310m, LastUpdated = DateTime.UtcNow },
                new StockCache { Symbol = "NICA",   SecurityName = "NIC Asia Bank Ltd.",             Sector = "Banking",          CurrentPrice = 512.3m,  Ltp = 512.3m,  PointChange = 18.4m,  PercentageChange = 3.73m,  PriceChange30d = 8.2m,  PeRatio = 14.2m, MarketCap = 42000000000, Volume = 245000, High52week = 580m,  Low52week = 390m,  Eps = 36.1m,  BookValue = 185m, LastUpdated = DateTime.UtcNow },
                new StockCache { Symbol = "GBIME",  SecurityName = "Global IME Bank Ltd.",           Sector = "Banking",          CurrentPrice = 376.8m,  Ltp = 376.8m,  PointChange = 9.8m,   PercentageChange = 2.67m,  PriceChange30d = 6.7m,  PeRatio = 16.3m, MarketCap = 56000000000, Volume = 421000, High52week = 420m,  Low52week = 285m,  Eps = 23.1m,  BookValue = 155m, LastUpdated = DateTime.UtcNow },
                new StockCache { Symbol = "SANIMA", SecurityName = "Sanima Bank Ltd.",               Sector = "Banking",          CurrentPrice = 298.5m,  Ltp = 298.5m,  PointChange = 5.5m,   PercentageChange = 1.88m,  PriceChange30d = 4.1m,  PeRatio = 13.8m, MarketCap = 33000000000, Volume = 198000, High52week = 340m,  Low52week = 240m,  Eps = 21.6m,  BookValue = 140m, LastUpdated = DateTime.UtcNow },
                new StockCache { Symbol = "UPPER",  SecurityName = "Upper Tamakoshi Hydropower Ltd.",Sector = "Hydropower",       CurrentPrice = 278.5m,  Ltp = 278.5m,  PointChange = 6.2m,   PercentageChange = 2.28m,  PriceChange30d = 5.4m,  PeRatio = 18.7m, MarketCap = 38000000000, Volume = 189000, High52week = 320m,  Low52week = 210m,  Eps = 14.9m,  BookValue = 98m,  LastUpdated = DateTime.UtcNow },
                new StockCache { Symbol = "SHIVM",  SecurityName = "Shiva Shree Hydropower Ltd.",    Sector = "Hydropower",       CurrentPrice = 215.0m,  Ltp = 215.0m,  PointChange = -4.5m,  PercentageChange = -2.05m, PriceChange30d = -3.1m, PeRatio = 22.1m, MarketCap = 4200000000,  Volume = 98000,  High52week = 265m,  Low52week = 180m,  Eps = 9.72m,  BookValue = 72m,  LastUpdated = DateTime.UtcNow },
                new StockCache { Symbol = "NIFRA",  SecurityName = "Nepal Infrastructure Bank",      Sector = "Development Bank", CurrentPrice = 98.7m,   Ltp = 98.7m,   PointChange = 3.1m,   PercentageChange = 3.24m,  PriceChange30d = 11.3m, PeRatio = 11.4m, MarketCap = 8500000000,  Volume = 312000, High52week = 115m,  Low52week = 72m,   Eps = 8.66m,  BookValue = 61m,  LastUpdated = DateTime.UtcNow },
                new StockCache { Symbol = "CHCL",   SecurityName = "Chilime Hydropower Co. Ltd.",    Sector = "Hydropower",       CurrentPrice = 540.0m,  Ltp = 540.0m,  PointChange = 10.0m,  PercentageChange = 1.89m,  PriceChange30d = 7.3m,  PeRatio = 20.5m, MarketCap = 22000000000, Volume = 145000, High52week = 620m,  Low52week = 430m,  Eps = 26.3m,  BookValue = 165m, LastUpdated = DateTime.UtcNow },
                new StockCache { Symbol = "NLG",    SecurityName = "Nepal Life Insurance Co. Ltd.",  Sector = "Insurance",        CurrentPrice = 1820.0m, Ltp = 1820.0m, PointChange = 25.0m,  PercentageChange = 1.39m,  PriceChange30d = 9.1m,  PeRatio = 24.1m, MarketCap = 28000000000, Volume = 62000,  High52week = 2100m, Low52week = 1450m, Eps = 75.5m,  BookValue = 420m, LastUpdated = DateTime.UtcNow },
                new StockCache { Symbol = "SICL",   SecurityName = "Siddartha Insurance Ltd.",       Sector = "Insurance",        CurrentPrice = 765.0m,  Ltp = 765.0m,  PointChange = 12.0m,  PercentageChange = 1.59m,  PriceChange30d = 5.8m,  PeRatio = 19.3m, MarketCap = 9800000000,  Volume = 88000,  High52week = 890m,  Low52week = 610m,  Eps = 39.6m,  BookValue = 235m, LastUpdated = DateTime.UtcNow },
            };

        foreach (var stock in sampleStocks)
        {
            await _stock.UpsertStockAsync(stock);
        }

        _logger.LogInformation("Seeded {Count} sample stocks into cache.", sampleStocks.Count);
    }
    private static string NormalizeSector(string raw) => raw?.Trim() switch
    {
        // Banks
        "Commercial Banks" => "Banking",
        "Commercial Bank" => "Banking",
        // Development Banks
        "Development Banks" => "Development Bank",
        "Development Bank" => "Development Bank",
        // Finance
        "Finance" => "Finance",
        "Finance Companies" => "Finance",
        // Hydropower — NEPSE uses "Hydro Power" (two words)
        "Hydro Power" => "Hydropower",
        "Hydropower" => "Hydropower",
        // Insurance — NEPSE splits into Life and Non-Life
        "Life Insurance" => "Insurance",
        "Non Life Insurance" => "Insurance",
        "Insurance" => "Insurance",
        // Microfinance
        "Microfinance" => "Microfinance",
        "Microfinance Promoter Shares" => "Microfinance",
        // Manufacturing
        "Manufacturing And Processing" => "Manufacturing",
        "Manufacturing" => "Manufacturing",
        // Others
        "Hotels And Tourism" => "Hotels & Tourism",
        "Trading" => "Trading",
        "Investment" => "Investment",
        "Others" => "Other",
        null or "" => "Other",
        var s => s,
    };
}