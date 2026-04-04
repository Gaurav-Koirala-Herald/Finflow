using System.Text.Json;
using System.Text.Json.Serialization;

public class NepseApiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };
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
        GetAsync<List<NepseSecurity>>("CompanyList");

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
        string baseUrl = _config["NepseApi:BaseUrl"]
            ?? throw new Exception("NepseApi:BaseUrl is not configured in appsettings.json");

        var response = await _http.GetAsync(baseUrl + endpoint);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json) || json == "null")
            throw new Exception($"Empty response from NEPSE endpoint: {endpoint}");

        return JsonSerializer.Deserialize<T>(json, _jsonOptions)
               ?? throw new Exception($"Failed to deserialize NEPSE response for: {endpoint}");
    }
}


public class NepseStockPrice
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("securityName")]
    public string SecurityName { get; set; } = string.Empty;

    [JsonPropertyName("securityId")]
    public string SecurityId { get; set; } = string.Empty;

    [JsonPropertyName("lastTradedPrice")]
    public decimal Ltp { get; set; }

    [JsonPropertyName("averageTradedPrice")]
    public decimal AverageTradedPrice { get; set; }

    [JsonPropertyName("highPrice")]
    public decimal HighPrice { get; set; }

    [JsonPropertyName("lowPrice")]
    public decimal LowPrice { get; set; }

    [JsonPropertyName("openPrice")]
    public decimal OpenPrice { get; set; }

    [JsonPropertyName("previousClose")]
    public decimal PreviousClose { get; set; }

    [JsonPropertyName("percentageChange")]
    public decimal PercentageChange { get; set; }

    [JsonPropertyName("totalTradeQuantity")]
    public long TotalTradeQuantity { get; set; }

    [JsonPropertyName("totalTradeValue")]
    public decimal TotalTradeValue { get; set; }

    [JsonPropertyName("lastTradedVolume")]
    public long LastTradedVolume { get; set; }

    [JsonPropertyName("lastUpdatedDateTime")]
    public string LastUpdatedDateTime { get; set; } = string.Empty;

    [JsonPropertyName("indexId")]
    public int IndexId { get; set; }

    // Computed: pointChange = LTP - previousClose (not returned by API directly)
    [JsonIgnore]
    public decimal PointChange => Ltp - PreviousClose;
}

public class NepseSecurity
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("securityName")]
    public string SecurityName { get; set; } = string.Empty;

    [JsonPropertyName("companyName")]
    public string CompanyName { get; set; } = string.Empty;

    [JsonPropertyName("sectorName")]
    public string SectorName { get; set; } = string.Empty;

    [JsonPropertyName("instrumentType")]
    public string InstrumentType { get; set; } = string.Empty;

    [JsonPropertyName("regulatoryBody")]
    public string RegulatoryBody { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public int Id { get; set; }

    // Resolved sector from sectorName field
    [JsonIgnore]
    public string Sector => SectorName;
}

public class NepseIndexEntry
{
    [JsonPropertyName("currentValue")]
    public decimal CurrentValue { get; set; }

    [JsonPropertyName("perChange")]
    public decimal PerChange { get; set; }

    [JsonPropertyName("absChange")]
    public decimal AbsChange { get; set; }
}

public class NepseStatus
{
    [JsonPropertyName("isOpen")]
    public string IsOpen { get; set; } = string.Empty;
}

public class NepseSummary
{
    [JsonPropertyName("Total Turnover Rs:")]
    public decimal TotalTurnover { get; set; }

    [JsonPropertyName("Total Transactions")]
    public double TotalTransactions { get; set; }

    [JsonPropertyName("Total Traded Shares")]
    public double TotalTradedShares { get; set; }

    [JsonPropertyName("decliningCount")]
    public int DecliningCount { get; set; }

    [JsonPropertyName("unchangedCount")]
    public int UnchangedCount { get; set; }
}

public class NepsePricePoint
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("closePrice")]
    public decimal ClosePrice { get; set; }

    [JsonPropertyName("volume")]
    public long Volume { get; set; }
}