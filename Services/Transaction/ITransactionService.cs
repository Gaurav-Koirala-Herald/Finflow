using FinflowAPI.Models;
using FinflowAPI.Models. Entities;
using FinflowAPI. Models.RequestModels.Transaction;
using FinflowAPI.Models.ResponseModels;
using FinflowAPI. Models.ResponseModels.Transaction;

namespace FinflowAPI.Services.Transaction;

public interface ITransactionService
{
    Task<ApiResponse<TransactionResponse>> CreateTransactionAsync(int userId, TransactionRequest request);
    Task<ApiResponse<TransactionResponse>> GetTransactionByIdAsync(int userId, int transactionId);
    Task<ApiResponse<PaginatedResponse<TransactionResponse>>> GetUserTransactionsAsync(int userId, TransactionFilterRequest filter);
    Task<ApiResponse<TransactionResponse>> UpdateTransactionAsync(int userId, int transactionId, TransactionRequest request);
    Task<ApiResponse<bool>> DeleteTransactionAsync(int userId, int transactionId);
    Task<ApiResponse<TransactionResponse>> GetTransactionSummaryAsync(int userId, DateTime? startDate, DateTime? endDate);
    Task<ApiResponse<List<TransactionCategory>>> GetCategoriesAsync(int?  transactionTypeId);
    Task<ApiResponse<List<TransactionType>>> GetTransactionTypesAsync();
}