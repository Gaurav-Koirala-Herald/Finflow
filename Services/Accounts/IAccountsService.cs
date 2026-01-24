using System.Security.Principal;

public interface IAccountsService
{
    Task<List<AccountDTO>> GetAllAccountsAsync(int userId);
    
    Task<AccountDTO> GetAccountDetailAsync(int userId,int accountId);

    Task<CommonResponseDTO> AddAccountAsync(AddAccountDTO addAccountDTO);
    Task<CommonResponseDTO> UpdateAccountAsync(UpdateAccountDTO updateAccountDTO);
    Task<CommonResponseDTO> DeleteAccountAsync(int userId,int accountId);
}