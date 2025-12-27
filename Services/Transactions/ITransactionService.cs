using RoleBaseAuthorization.DTO;

namespace RoleBaseAuthorization.Services.Transactions;

public interface ITransactionService
{
    Task<List<TransactionDto>> GetAllTransactionAsync(int userId);
    Task<bool> CreateTransactionAsync(TransactionDto dto);
    Task<bool> UpdateTransactionAsync(TransactionDto dto);
    Task<bool> DeleteTransactionAsync(int id);
}