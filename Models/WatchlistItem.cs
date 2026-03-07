
namespace FinFlowAPI.Models;

public class WatchlistItem
{
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public decimal? AlertPrice { get; set; }
        public DateTime AddedAt { get; set; }
}