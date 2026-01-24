using FinFlowAPI.Models;

namespace FinFlowAPI.DTO;

public class TransactionCategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long transactionTypeId { get; set; }
}