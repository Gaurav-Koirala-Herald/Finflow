using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        int estimatedTokens = Math.Max(1024, picks.Count * 200 + 512);
        int maxTokens = Math.Min(estimatedTokens, 8192);

        string prompt = $@"You are a NEPSE (Nepal Stock Exchange) investment advisor.

INVESTOR PROFILE:
- Risk Tolerance: {user.RiskLevel}
- Preferred Sectors: {string.Join(", ", user.PreferredSectors)}
- Investment Amount: Rs.{user.InvestmentAmount:N0}

RECOMMENDED STOCKS:
{string.Join("\n", stockLines)}

For EACH stock write 2-3 sentences covering:
1. Why it fits this investor's profile
2. Key fundamental or technical reason
3. One specific risk to watch

Respond ONLY as a valid JSON array. No markdown, no code fences, no extra text.
Use this exact format:
[{{""symbol"":""NICA"",""explanation"":""Your explanation here.""}}]";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                temperature = 0.4,           
                maxOutputTokens = maxTokens,
                responseMimeType = "application/json"  // Force JSON mode on Gemini
            }
        };

        var response = await _http.PostAsJsonAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}",
            requestBody);

        response.EnsureSuccessStatusCode();

        var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();
        string rawText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "[]";

        return ParseExplanations(rawText);
    }

    private static Dictionary<string, string> ParseExplanations(string raw)
    {
        try
        {
            string json = CleanJson(raw);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            try
            {
                var items = JsonSerializer.Deserialize<List<GeminiExplanationItem>>(json, options);
                if (items != null && items.Count > 0)
                    return items.ToDictionary(i => i.Symbol, i => i.Explanation);
            }
            catch (JsonException) { }

            string repaired = RepairTruncatedJson(json);
            var repairedItems = JsonSerializer.Deserialize<List<GeminiExplanationItem>>(repaired, options);

            return repairedItems?.ToDictionary(i => i.Symbol, i => i.Explanation)
                   ?? new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[GeminiService] JSON parse failed: {ex.Message}");
            return new Dictionary<string, string>();
        }
    }

    private static string CleanJson(string raw)
    {
        raw = Regex.Replace(raw, @"```(?:json)?", "", RegexOptions.IgnoreCase).Trim();

        var match = Regex.Match(raw, @"\[.*\]", RegexOptions.Singleline);
        if (match.Success)
            return match.Value;

        var objMatch = Regex.Match(raw, @"\{.*\}", RegexOptions.Singleline);
        if (objMatch.Success)
            return $"[{objMatch.Value}]";

        return raw;
    }

    private static string RepairTruncatedJson(string json)
    {
        var sb = new StringBuilder(json.TrimEnd());

        int quoteCount = json.Count(c => c == '"');
        if (quoteCount % 2 != 0)
            sb.Append('"');

        var stack = new Stack<char>();
        bool inString = false;

        for (int i = 0; i < json.Length; i++)
        {
            char c = json[i];
            if (c == '"' && (i == 0 || json[i - 1] != '\\'))
                inString = !inString;

            if (!inString)
            {
                if (c == '{') stack.Push('}');
                else if (c == '[') stack.Push(']');
                else if ((c == '}' || c == ']') && stack.Count > 0)
                    stack.Pop();
            }
        }

        while (stack.Count > 0)
            sb.Append(stack.Pop());

        return sb.ToString();
    }
}

// Gemini API response models
public class GeminiResponse
{
    public List<GeminiCandidate> Candidates { get; set; } = new();
}

public class GeminiCandidate
{
    public GeminiContent Content { get; set; } = new();
    public string? FinishReason { get; set; }  // "STOP", "MAX_TOKENS", etc.
}

public class GeminiContent
{
    public List<GeminiPart> Parts { get; set; } = new();
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