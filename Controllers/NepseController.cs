using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoleBaseAuthorization.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class NepseController : ControllerBase
{
    [HttpGet("TopLosers")]
    public async Task<IActionResult> GetTopLosers()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync("https://nepseapi.surajrimal.dev/TopLosers");
        return Content(response,"application/json");
    }
    [HttpGet("NepseIndex")]
    public async Task<IActionResult> GetNepseIndex()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync("https://nepseapi.surajrimal.dev/NepseIndex");
        return Content(response,"application/json");

    }
    [HttpGet("TopGainers")]
    public async Task<IActionResult> GetTopGainers()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync("https://nepseapi.surajrimal.dev/TopGainers");
        return Content(response,"application/json");

    }
    [HttpGet("IsNepseOpen")]
    public async Task<IActionResult> IsNepseOpen()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync("https://nepseapi.surajrimal.dev/IsNepseOpen");
        return Content(response,"application/json");

    }
   
}