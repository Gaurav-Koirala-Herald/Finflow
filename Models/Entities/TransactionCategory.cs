namespace FinflowAPI.Models. Entities;

public class TransactionCategory
{
    public int CategoryId { get; set; }
    public int TransactionTypeId { get; set; }
    public string CategoryName { get; set; } = string. Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime. UtcNow;

    // Navigation Properties
    public virtual TransactionType TransactionType { get; set; } = null! ;
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}