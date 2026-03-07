using System.Text.Json;
using FinFlowAPI.DTO;

public class GeminiService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public GeminiService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Gemini:ApiKey"]
            ?? throw new Exception("Gemini API key missing in appsettings.json");
    }

    public async Task<Dictionary<string, string>> GetExplanationsAsync(
        UserDto user,
        List<(StockCache Stock, decimal Score, ScoreBreakdown Breakdown)> picks)
    {
        var stockLines = picks.Select(p =>
            $"- {p.Stock.Symbol} ({p.Stock.Sector}) " +
            $"| Price: Rs.{p.Stock.CurrentPrice} " +
            $"| 30d Change: {p.Stock.PriceChange30d:+0.00;-0.00}% " +
            $"| P/E: {p.Stock.PeRatio} " +
            $"| AI Score: {p.Score:P0}");

        string prompt = $@"
You are a NEPSE (Nepal Stock Exchange) investment advisor.

INVESTOR PROFILE:
- Risk Tolerance: {user.RiskLevel}
- Preferred Sectors: {string.Join(", ", user.PreferredSectors)}
- Investment Amount: Rs.{user.InvestmentAmount:N0}

RECOMMENDED STOCKS:
{string.Join("\n", stockLines)}

For EACH stock write 2-3 sentences:
1. Why it fits this investor's profile
2. Key fundamental or technical reason to consider it
3. One specific risk to watch out for

End with a brief general risk disclaimer.

Respond ONLY as a JSON array. No markdown, no extra text. Example:
[{{ ""symbol"": ""NICA"", ""explanation"": ""..."" }}]
";

        var requestBody = new
        {
            contents = new[]
            {
                    new { parts = new[] { new { text = prompt } } }
                },
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 2048
            }
        };

        var response = await _http.PostAsJsonAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}",
            requestBody);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<GeminiResponse>();
        string text = json?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "[]";

        return ParseExplanations(text);
    }

    private static Dictionary<string, string> ParseExplanations(string json)
    {
        try
        {
            json = json.Trim().TrimStart('`');
            if (json.StartsWith("json")) json = json[4..];
            json = json.TrimEnd('`').Trim();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer.Deserialize<List<GeminiExplanationItem>>(json, options);

            return items?.ToDictionary(i => i.Symbol, i => i.Explanation)
                   ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}

// Gemini API response models
public class GeminiResponse
{
    public List<GeminiCandidate> Candidates { get; set; } = new List<GeminiCandidate>();
}

public class GeminiCandidate
{
    public GeminiContent Content { get; set; } = new GeminiContent();
}

public class GeminiContent
{
    public List<GeminiPart> Parts { get; set; } = new List<GeminiPart>();
}

public class GeminiPart
{
    public string Text { get; set; } = string.Empty;
}

public class GeminiExplanationItem
{
    public string Symbol { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}