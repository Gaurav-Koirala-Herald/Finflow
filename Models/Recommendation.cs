
namespace FinFlowAPI.Models;

public class Recommendation
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
}