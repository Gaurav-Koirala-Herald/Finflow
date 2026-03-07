public class NepseApiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    public NepseApiService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public Task<List<NepseStockPrice>> GetLiveMarketAsync() =>
        GetAsync<List<NepseStockPrice>>("LiveMarket");

    public Task<List<NepseStockPrice>> GetTopGainersAsync() =>
        GetAsync<List<NepseStockPrice>>("TopGainers");

    public Task<List<NepseStockPrice>> GetTopLosersAsync() =>
        GetAsync<List<NepseStockPrice>>("TopLosers");

    public Task<List<NepseSecurity>> GetSecurityListAsync() =>
        GetAsync<List<NepseSecurity>>("SecurityList");

    public Task<Dictionary<string, NepseIndexEntry>> GetNepseIndexAsync() =>
        GetAsync<Dictionary<string, NepseIndexEntry>>("NepseIndex");

    public Task<NepseStatus> GetNepseStatusAsync() =>
        GetAsync<NepseStatus>("IsNepseOpen");

    public Task<NepseSummary> GetSummaryAsync() =>
        GetAsync<NepseSummary>("Summary");

    public Task<List<NepsePricePoint>> GetDailyPriceGraphAsync(string symbol) =>
        GetAsync<List<NepsePricePoint>>($"DailyScripPriceGraph?symbol={symbol}");

    private async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _http.GetAsync(_config["NepseApi:BaseUrl"] + endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>()
               ?? throw new Exception($"Empty response from NEPSE endpoint: {endpoint}");
    }
}


public class NepseStockPrice
{
    public string Symbol { get; set; } = string.Empty;
    public string SecurityName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public decimal Ltp { get; set; }
    public decimal PointChange { get; set; }
    public decimal PercentageChange { get; set; }
    public long TotalTradeQuantity { get; set; }
    public decimal TotalTradeValue { get; set; }
    public decimal OpenPrice { get; set; }
    public decimal HighPrice { get; set; }
    public decimal LowPrice { get; set; }
    public decimal PreviousClose { get; set; }
}

public class NepseSecurity
{
    public string Symbol { get; set; } = string.Empty;
    public string SecurityName { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
}

public class NepseIndexEntry
{
    public decimal CurrentValue { get; set; }
    public decimal PerChange { get; set; }
    public decimal AbsChange { get; set; }
}

public class NepseStatus
{
    public string IsOpen { get; set; } = string.Empty;
}

public class NepseSummary
{
    public decimal TotalTurnover { get; set; }
    public int TotalTransactions { get; set; }
    public int AdvancingCount { get; set; }
    public int DecliningCount { get; set; }
    public int UnchangedCount { get; set; }
}

public class NepsePricePoint
{
    public string Date { get; set; } = string.Empty;
    public decimal ClosePrice { get; set; }
    public long Volume { get; set; }
}