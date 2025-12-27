namespace FinflowAPI.Models.ResponseModels.Transaction;

public class TransactionResponse
{
    public int TransactionId { get; set; }
    public int UserId { get; set; }
    public int TransactionTypeId { get; set; }
    public string TransactionTypeName { get; set; } = string. Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string?  CategoryIcon { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string?  Description { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}