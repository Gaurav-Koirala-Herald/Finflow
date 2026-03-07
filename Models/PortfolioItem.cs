namespace FinFlowAPI.Models;
public class PortfolioItem
{
      public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal BuyPrice { get; set; }
        public DateTime BoughtAt { get; set; }
}