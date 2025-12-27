using RoleBaseAuthorization.Models;

public class TransactionCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}