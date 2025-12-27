using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoleBaseAuthorization.DTO;
using RoleBaseAuthorization.Models;
using RoleBaseAuthorization.Services;

namespace RoleBaseAuthorization.Controllers;

[Authorize]
[ApiController]
[Route("api/select-list")]
public class CommonController(CommonService commonService) : ControllerBase
{
    [HttpGet("transaction-types")]
    public async Task<List<TransactionTypeDTO>> GetTransactionTypes()
    {
        var result = await commonService.GetTransactionType();
        var mappedResponse = result.Adapt<List<TransactionTypeDTO>>();
        return mappedResponse;
    }

    [HttpGet("transaction-category")]
    public async Task<List<TransactionCategoryDTO>> GetTransactionCategory()
    {
        var result = await commonService.GetTransactionCategory();
        var mappedResponse = result.Adapt<List<TransactionCategoryDTO>>();
        return mappedResponse;
    }
}