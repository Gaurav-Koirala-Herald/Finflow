
namespace FinFlowAPI.Models;

public class UserSector
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}
