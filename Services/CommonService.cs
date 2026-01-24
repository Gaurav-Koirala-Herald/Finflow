using Microsoft.EntityFrameworkCore;
using FinFlowAPI.Data;
using FinFlowAPI.Models;
using Mapster;
using FinFlowAPI.DTO;
using Dapper;
using Microsoft.OpenApi.Models;
using System.Diagnostics;

namespace FinFlowAPI.Services;

public class CommonService(ApplicationDbContext dbContext,SqlHandlerService handler)
{
    public async Task<List<TransactionType>> GetTransactionType()
    {
        return await dbContext.TransactionTypes.ToListAsync();
    }
    
    public async Task<List<TransactionCategoryDTO>> GetTransactionCategory()
    {
        var response =  await dbContext.TransactionCategories.ToListAsync();
        var mappedResponse = response.Adapt<List<TransactionCategoryDTO>>();
        return mappedResponse;
    }
    public async Task<List<AccountType>> GetAccountTypesAsync()
    {
        string sp = "proc_get_account_types";
        var param = new DynamicParameters();
        var dbResp = await handler.ExecuteAsyncList<AccountType>(sp,param);
        return dbResp;
    }
}