
using Dapper;
using FinFlowAPI.Models;
using FinFlowAPI.Services;

public class AccountService : IAccountsService
{
    private readonly SqlHandlerService _handler;
    public AccountService(SqlHandlerService handler)
    {
        _handler = handler;        
    }
    public async Task<CommonResponseDTO> AddAccountAsync(AddAccountDTO addAccountDTO)
    {
        string sp ="proc_add_account";
        var param = new DynamicParameters();
        param.Add("@userId",addAccountDTO.userId);
        param.Add("@accountTypeId",addAccountDTO.accountTypeId);
        param.Add("@accountBalance",addAccountDTO.accountBalance);
        
        var dbResp = await _handler.ExecuteAsync<CommonResponseDTO>(sp,param);
        return dbResp;
    }

    public async Task<CommonResponseDTO> DeleteAccountAsync(int userId, int accountId)
    {
        string sp = "proc_delete_account";
        var param = new DynamicParameters();
        param.Add("@userId",userId);
        param.Add("@accountId",accountId);

        var dbResp = await _handler.ExecuteAsync<CommonResponseDTO>(sp,param);
        return dbResp;
    }

    public async Task<AccountDTO> GetAccountDetailAsync(int userId, int accountId)
    {
        string sp = "proc_get_account_details";
        var param = new DynamicParameters();
        param.Add("@userId",userId);
        param.Add("@accountId",accountId);

        var dbResp = await _handler.ExecuteAsync<AccountDTO>(sp,param);
        return dbResp;
    }

    public async Task<List<AccountDTO>> GetAllAccountsAsync(int userId)
    {
        string sp = "proc_get_all_accounts";
        var param = new DynamicParameters();
        param.Add("@userId",userId);
        var dbResp = await _handler.ExecuteAsyncList<AccountDTO>(sp,param);
        return dbResp;
    }

    public async Task<CommonResponseDTO> UpdateAccountAsync(UpdateAccountDTO updateAccountDTO)
    {
        string sp = "proc_update_account";
        var param = new DynamicParameters();
        param.Add("@accountId",updateAccountDTO.id);
        param.Add("@userId",updateAccountDTO.userId);
        param.Add("@accountTypeId",updateAccountDTO.accountTypeId);
        param.Add("@accountBalance",updateAccountDTO.accountBalance);

        var dbResp = await _handler.ExecuteAsync<CommonResponseDTO>(sp,param);
        return dbResp;
    }
}