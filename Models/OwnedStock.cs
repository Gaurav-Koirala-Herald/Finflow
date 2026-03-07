namespace FinFlowAPI.Models;
public class OwnedStock
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}