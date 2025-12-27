namespace FinflowAPI.Models. Entities;

public class TransactionType
{
    public int TransactionTypeId { get; set; }
    public string TypeName { get; set; } = string. Empty;
    public string?  Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<TransactionCategory> TransactionCategories { get; set; } = new List<TransactionCategory>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}