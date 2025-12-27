using Microsoft.EntityFrameworkCore;
using FinFlowAPI.Data;
using FinFlowAPI.Models;

namespace FinFlowAPI.Services;

public class CommonService(ApplicationDbContext dbContext)
{
    public async Task<List<TransactionType>> GetTransactionType()
    {
        return await dbContext.TransactionTypes.ToListAsync();
    }
    
    public async Task<List<TransactionCategory>> GetTransactionCategory()
    {
        return await dbContext.TransactionCategories.ToListAsync();
    }
}