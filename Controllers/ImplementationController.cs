using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FinflowAPI.Controllers;
[Microsoft.AspNetCore.Components.Route("api")]
[Authorize]
public class ImplementationController : BaseApiController
{
    [HttpGet("authorize")]
    public IActionResult AuthorizeMethod()
    {
        return Ok("Used Authorize Method");
    }
}