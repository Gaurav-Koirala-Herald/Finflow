namespace FinflowAPI.Models.Entities;

public class Transaction
{
    public int TransactionId { get; set; }
    public int UserId { get; set; }
    public int TransactionTypeId { get; set; }
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string?  Description { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual TransactionType TransactionType { get; set; } = null!;
    public virtual TransactionCategory Category { get; set; } = null!;
}