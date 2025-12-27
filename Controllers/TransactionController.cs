using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinFlowAPI.DTO;
using FinFlowAPI.Services.Transactions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FinFlowAPI.Controllers;

[Authorize]
[Route("api/transactions")]
[ApiController]
public class TransactionController(ITransactionService _service) : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetAllTransactions(int userId)
    {
        var result = await _service.GetAllTransactionAsync(userId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(TransactionDto dto)
    {
        return Ok(await _service.CreateTransactionAsync(dto));
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(TransactionDto dto)
    {
        return Ok(await _service.UpdateTransactionAsync(dto));
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        return Ok(await _service.DeleteTransactionAsync(id));
    }
}