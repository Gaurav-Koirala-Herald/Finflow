namespace FinflowAPI.Models.RequestModels.Transaction;

public class TransactionFilterRequest
{
    public int? TransactionTypeId { get; set; }
    public int? CategoryId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "TransactionDate";
    public bool SortDescending { get; set; } = true;
}