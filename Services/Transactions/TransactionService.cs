using Mapster;
using Microsoft.EntityFrameworkCore;
using FinFlowAPI.Data;
using FinFlowAPI.DTO;
using FinFlowAPI.Models;

namespace FinFlowAPI.Services.Transactions;

public class TransactionService(ApplicationDbContext dbContext) : ITransactionService
{
    public async Task<List<TransactionDto>> GetAllTransactionAsync(int userId)
    {
        var transactions =await  dbContext.Transactions.Where(x => x.userId == userId && x.IsActive)
        .Select(x=> new TransactionDto
        {
        Id = x.Id,
        Name = x.Name,
        Description = x.Description,
        categoryId = x.categoryId,
        transactionTypeId = x.transactionTypeId,
        Amount = x.Amount,
        TransactionDate = x.TransactionDate.Date, 
        userId = x.userId
        }).ToListAsync();
        return transactions;
    }

    public async Task<bool> CreateTransactionAsync(TransactionDto dto)
    {
        var mappedRequest = dto.Adapt<Transaction>();
        mappedRequest.IsActive = true;
        await dbContext.Transactions.AddAsync(mappedRequest);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTransactionAsync(TransactionDto dto)
    {
        var existingTransactions = await dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == dto.Id);
        
        if(existingTransactions == null) return (false);
        existingTransactions.userId = dto.userId;
        existingTransactions.Amount = dto.Amount;
        existingTransactions.Description = dto.Description;
        existingTransactions.TransactionDate = dto.TransactionDate;
        existingTransactions.transactionTypeId= dto.transactionTypeId;
        existingTransactions.userId = dto.userId;
        existingTransactions.IsActive = true;
        existingTransactions.TransactionDate = dto.TransactionDate;
        existingTransactions.categoryId = dto.categoryId;
        existingTransactions.Name = dto.Name;
        
        dbContext.Transactions.Update(existingTransactions);
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