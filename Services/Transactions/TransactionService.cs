using Mapster;
using Microsoft.EntityFrameworkCore;
using RoleBaseAuthorization.Data;
using RoleBaseAuthorization.DTO;
using RoleBaseAuthorization.Models;

namespace RoleBaseAuthorization.Services.Transactions;

public class TransactionService(ApplicationDbContext dbContext) : ITransactionService
{
    public async Task<List<TransactionDto>> GetAllTransactionAsync(int userId)
    {
        var transactions =await  dbContext.Transactions.Where(x => x.userId == userId).ToListAsync();
        var mappedResponse = transactions.Adapt<List<TransactionDto>>();
        return mappedResponse;
    }

    public async Task<bool> CreateTransactionAsync(TransactionDto dto)
    {
        var mappedRequest = dto.Adapt<Transaction>();
        await dbContext.Transactions.AddAsync(mappedRequest);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTransactionAsync(TransactionDto dto)
    {
        var existingTransactions = await dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == dto.Id);
        
        if(existingTransactions == null) return (false);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        var existingTransaction = await dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == id);
        if(existingTransaction == null) return (false);
        
        existingTransaction.IsActive = false;
        dbContext.Transactions.Update(existingTransaction);
        return await dbContext.SaveChangesAsync() > 0;
    }
}