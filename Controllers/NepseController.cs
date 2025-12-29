using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinFlowAPI.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class NepseController : ControllerBase
{
    [HttpGet("TopLosers")]
    public async Task<IActionResult> GetTopLosers()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync("https://nepse-api-eight.vercel.app/top-losers");
        return Content(response,"application/json");
    }
    [HttpGet("NepseIndex")]
    public async Task<IActionResult> GetNepseIndex()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync("https://nepse-api-eight.vercel.app/nepse-index");
        return Content(response,"application/json");

    }
    [HttpGet("TopGainers")]
    public async Task<IActionResult> GetTopGainers()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync("https://nepse-api-eight.vercel.app/top-gainers");
        return Content(response,"application/json");

    }
    [HttpGet("IsNepseOpen")]
    public async Task<IActionResult> IsNepseOpen()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync("https://nepse-api-eight.vercel.app/is-nepse-open");
        return Content(response,"application/json");

    }
   
}