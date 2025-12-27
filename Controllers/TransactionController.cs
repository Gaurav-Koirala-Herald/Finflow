using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinflowAPI.Models;
using FinflowAPI.Models.RequestModels. Transaction;
using FinflowAPI. Services.Transaction;
using FinflowAPI.Services.Transaction;

namespace FinflowAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Validation failed",
                ModelState.Values. SelectMany(v => v.Errors. Select(e => e.ErrorMessage)).ToList()));
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user"));
        }

        var result = await _transactionService.CreateTransactionAsync(userId.Value, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetTransaction), new { transactionId = result. Data?. TransactionId }, result);
    }

    [HttpGet("{transactionId}")]
    public async Task<IActionResult> GetTransaction(int transactionId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user"));
        }

        var result = await _transactionService. GetTransactionByIdAsync(userId.Value, transactionId);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] TransactionFilterRequest filter)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user"));
        }

        var result = await _transactionService.GetUserTransactionsAsync(userId.Value, filter);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{transactionId}")]
    public async Task<IActionResult> UpdateTransaction(int transactionId, [FromBody] TransactionRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user"));
        }

        var result = await _transactionService. UpdateTransactionAsync(userId.Value, transactionId, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{transactionId}")]
    public async Task<IActionResult> DeleteTransaction(int transactionId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user"));
        }

        var result = await _transactionService.DeleteTransactionAsync(userId. Value, transactionId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetTransactionSummary(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user"));
        }

        var result = await _transactionService.GetTransactionSummaryAsync(userId.Value, startDate, endDate);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories([FromQuery] int?  transactionTypeId = null)
    {
        var result = await _transactionService.GetCategoriesAsync(transactionTypeId);

        if (!result. Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetTransactionTypes()
    {
        var result = await _transactionService.GetTransactionTypesAsync();

        if (!result. Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User. FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int. TryParse(userIdClaim, out int userId) ? userId : null;
    }
}