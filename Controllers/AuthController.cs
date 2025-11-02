using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class AuthController : BaseApiController
{
    [AllowAnonymous]
    [Route("api/login")]
    public async Task<IActionResult> login()
    {
        var loginResponse = "To be implemented";
        return Ok(loginResponse);
    }

    [Authorize]
    [Route("api/refresh")]
    public async Task<IActionResult> refersh()
    {
        var refreshToken = "the refresh token has been invoked";
        return Ok();
    }

    [Authorize]
    [Route("api/logout")]
    public async Task<IActionResult> logout()
    {
        var logoutResponse = "To be implemented";
        return Ok(logoutResponse);
    }
}