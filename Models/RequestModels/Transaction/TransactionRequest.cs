using System.ComponentModel.DataAnnotations;

namespace FinflowAPI.Models.RequestModels.Transaction;

public class TransactionRequest
{
    [Required]
    public int TransactionTypeId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime TransactionDate { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? PaymentMethod { get; set; }
}