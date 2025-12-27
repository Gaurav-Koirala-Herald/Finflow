using FinflowAPI.Services.Transaction;
using FinflowAPI.DbImplementation. Interfaces;
using FinflowAPI.Models;
using FinflowAPI.Models.Entities;
using FinflowAPI.Models.RequestModels. Transaction;
using FinflowAPI.Models.ResponseModels;
using FinflowAPI. Models.ResponseModels. Transaction;
using FinflowAPI. Services.Transaction;

namespace FinflowAPI.Services. Implementations;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(IUnitOfWork unitOfWork, ILogger<TransactionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<TransactionResponse>> CreateTransactionAsync(int userId, TransactionRequest request)
    {
        try
        {
            // if (! await _unitOfWork.TransactionTypes.IsValidTypeAsync(request.TransactionTypeId))
            // {
            //     return ApiResponse<TransactionResponse>. FailureResponse("Invalid transaction type");
            // }
            //
            // if (!await _unitOfWork.TransactionCategories.IsValidCategoryForTypeAsync(request. CategoryId, request.TransactionTypeId))
            // {
            //     return ApiResponse<TransactionResponse>.FailureResponse("Invalid category for the selected transaction type");
            // }
            //
            var transaction = new Models.Entities.Transaction
            {
                UserId = userId,
                TransactionTypeId = request.TransactionTypeId,
                CategoryId = request.CategoryId,
                Amount = request. Amount,
                TransactionDate = request.TransactionDate,
                Description = request.Description?. Trim(),
                PaymentMethod = request. PaymentMethod?. Trim(),
                CreatedAt = DateTime. UtcNow
            };
            
            // var createdTransaction = await _unitOfWork. Transactions.CreateTransactionAsync(transaction);

            return ApiResponse<TransactionResponse>.SuccessResponse(
                new TransactionResponse {TransactionId = transaction.TransactionId},
                "Transaction created successfully");
        }
        catch (Exception ex)
        {
            _logger. LogError(ex, "Error creating transaction for user:  {UserId}", userId);
            return ApiResponse<TransactionResponse>.FailureResponse("An error occurred while creating transaction");
        }
    }

    public async Task<ApiResponse<TransactionResponse>> GetTransactionByIdAsync(int userId, int transactionId)
    {
        try
        {
            // var transaction = await _unitOfWork.Transactions.GetByIdWithDetailsAsync(transactionId, userId);
    
            // if (transaction == null)
            // {
            //     return ApiResponse<TransactionResponse>.FailureResponse("Transaction not found");
            // }
    
            return ApiResponse<TransactionResponse>. SuccessResponse(new TransactionResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction {TransactionId} for user {UserId}", transactionId, userId);
            return ApiResponse<TransactionResponse>.FailureResponse("An error occurred while fetching transaction");
        }
    }

    public async Task<ApiResponse<PaginatedResponse<TransactionResponse>>> GetUserTransactionsAsync(
        int userId, TransactionFilterRequest filter)
    {
        try
        {
            // var result = await _unitOfWork.Transactions. GetUserTransactionsPagedAsync(userId, filter);
            return ApiResponse<PaginatedResponse<TransactionResponse>>.SuccessResponse(new PaginatedResponse<TransactionResponse>(), "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transactions for user:  {UserId}", userId);
            return ApiResponse<PaginatedResponse<TransactionResponse>>.FailureResponse(
                "An error occurred while fetching transactions");
        }
    }

    public async Task<ApiResponse<TransactionResponse>> UpdateTransactionAsync(
        int userId, int transactionId, TransactionRequest request)
    {
        try
        {
            // var transaction = await _unitOfWork.Transactions.GetByIdWithDetailsAsync(transactionId, userId);
    
            // if (transaction == null)
            // {
            //     return ApiResponse<TransactionResponse>. FailureResponse("Transaction not found");
            // }
            //
            //
            // transaction.UpdatedAt = DateTime.UtcNow;
            //
            // _unitOfWork. Transactions.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
            
            // transaction = await _unitOfWork.Transactions. GetByIdWithDetailsAsync(transactionId, userId);
    
            return ApiResponse<TransactionResponse>.SuccessResponse(
                new TransactionResponse(),"Transaction updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction {TransactionId} for user {UserId}", transactionId, userId);
            return ApiResponse<TransactionResponse>.FailureResponse("An error occurred while updating transaction");
        }
    }

    public async Task<ApiResponse<bool>> DeleteTransactionAsync(int userId, int transactionId)
    {
        try
        {
            // var transaction = await _unitOfWork. Transactions.GetByIdWithDetailsAsync(transactionId, userId);

            // if (transaction == null)
            // {
            //     return ApiResponse<bool>.FailureResponse("Transaction not found");
            // }
            //
            // _unitOfWork.Transactions. Remove(transaction);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>. SuccessResponse(true, "Transaction deleted successfully");
        }
        catch (Exception ex)
        {
            _logger. LogError(ex, "Error deleting transaction {TransactionId} for user {UserId}", transactionId, userId);
            return ApiResponse<bool>. FailureResponse("An error occurred while deleting transaction");
        }
    }

    public Task<ApiResponse<TransactionResponse>> GetTransactionSummaryAsync(int userId, DateTime? startDate, DateTime? endDate)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<List<TransactionCategory>>> GetCategoriesAsync(int? transactionTypeId)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<List<TransactionType>>> GetTransactionTypesAsync()
    {
        throw new NotImplementedException();
    }

    // public async Task<ApiResponse<TransactionSummaryResponse>> GetTransactionSummaryAsync(
    //     int userId, DateTime? startDate, DateTime? endDate)
    // {
    //     try
    //     {
    //         var summary = await _unitOfWork.Transactions. GetTransactionSummaryAsync(userId, startDate, endDate);
    //         return ApiResponse<TransactionSummaryResponse>.SuccessResponse(summary);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting transaction summary for user:  {UserId}", userId);
    //         return ApiResponse<TransactionSummaryResponse>. FailureResponse(
    //             "An error occurred while fetching transaction summary");
    //     }
    // }

    // public async Task<ApiResponse<List<TransactionCategory>>> GetCategoriesAsync(int?  transactionTypeId)
    // {
    //     try
    //     {
    //         var categories = await _unitOfWork.TransactionCategories.GetActiveCategoriesAsync(transactionTypeId);
    //         return ApiResponse<List<TransactionCategory>>.SuccessResponse(categories);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting categories");
    //         return ApiResponse<List<TransactionCategory>>.FailureResponse(
    //             "An error occurred while fetching categories");
    //     }
    // }

    // public async Task<ApiResponse<List<TransactionType>>> GetTransactionTypesAsync()
    // {
    //     try
    //     {
    //         var types = await _unitOfWork.TransactionTypes.GetActiveTypesAsync();
    //         return ApiResponse<List<TransactionType>>.SuccessResponse(types);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting transaction types");
    //         return ApiResponse<List<TransactionType>>. FailureResponse(
    //             "An error occurred while fetching transaction types");
    //     }
    // }

    // private static TransactionResponse MapToTransactionResponse(Transaction transaction)
    // {
    //     return new TransactionResponse
    //     {
    //         TransactionId = transaction.TransactionId,
    //         UserId = transaction. UserId,
    //         TransactionTypeId = transaction.TransactionTypeId,
    //         TransactionTypeName = transaction.TransactionType. TypeName,
    //         CategoryId = transaction.CategoryId,
    //         CategoryName = transaction.Category.CategoryName,
    //         CategoryIcon = transaction.Category. Icon,
    //         Amount = transaction.Amount,
    //         TransactionDate = transaction.TransactionDate,
    //         Description = transaction.Description,
    //         PaymentMethod = transaction.PaymentMethod,
    //         CreatedAt = transaction.CreatedAt,
    //         UpdatedAt = transaction.UpdatedAt
    //     };
    // }
 }
