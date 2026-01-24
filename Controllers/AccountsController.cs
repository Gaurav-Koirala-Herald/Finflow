using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinFlowAPI.DTO;
using FinFlowAPI.Services.Auth;
using System.Security.Permissions;
using System.CodeDom.Compiler;

namespace FinFlowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _account;

        public AccountsController(IAccountsService account)
        {
            _account = account;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllAccountsAsync(int userId)
        {
            var response = await _account.GetAllAccountsAsync(userId);
            if (response == null)
            {
                return Ok(new
                {
                    code = 204,
                    message = "Record not found"
                });
            }
            return Ok(new
            {
                code = 200,
                message = "Accounts fetched Successfully",
                data = response
            });
        }
        [HttpPost]
        public async Task<IActionResult> AddAccountsAsync(AddAccountDTO addAccountDTO)
        {
            var response = await _account.AddAccountAsync(addAccountDTO);
            if (response.code != 0)
            {
                return BadRequest(new
                {
                    response.code,
                    response.message
                });
            }
            return Ok(new
            {
                code = 200,
                message = response.message
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAccountsAsync(UpdateAccountDTO updateAccountDTO)
        {
            var response = await _account.UpdateAccountAsync(updateAccountDTO);

            if(response.code != 0)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = response.message
                });
            }

            return Ok(new
            {
               code = 200,
               response.message 
            });

        }
        [HttpGet("account-details")]
        public async Task<IActionResult> GetAccountDetailsAsync(int accountId,int userId)
        {
            var response = await _account.GetAccountDetailAsync(userId,accountId);
            if(response == null)
            {
                return Ok(new
                {
                    code = 204,
                    message = "Empty Result Set"
                });
            }
            return Ok(new
            {
                code = 200,
                message = "Record Fetched",
                data = response
            });
        }
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccountAsync(int userId,int accountId)
        {
            var response  = await _account.DeleteAccountAsync(userId,accountId);
            if(response.code != 0)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = response.message
                });
            }
            return Ok(new
            {
                code = 200,
                message = response.message
            });
        }
    }
}