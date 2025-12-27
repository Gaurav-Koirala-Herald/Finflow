namespace FinFlowAPI.DTO;

public class TransactionDto
{
    public int Id { get; set; }
    public int userId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int categoryId { get; set; }
    public int transactionTypeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
}