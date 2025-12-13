using System;
using FinflowAPI.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[Route("api")]
[ApiController]
public class AuthController(IJwtTokenGenerator jwtTokenGenerator) : BaseApiController
{
    [AllowAnonymous]
    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> login(LoginRequest loginRequest)
    {
        if (loginRequest.userName == "gaurav" && loginRequest.password == "pwd")
        {
            var resposnse = jwtTokenGenerator.GenerateAccessToken(loginRequest);
            ;
            return Ok(resposnse);
        }
        return Unauthorized();
    }

    [Authorize]
    [HttpPut]
    [Route("refresh")]
    public async Task<IActionResult> refersh()
    {
        var refreshToken = "the refresh token has been invoked";
        return Ok();
    }

    [Authorize]
    [HttpDelete]
    [Route("logout")]
    public async Task<IActionResult> logout()
    {
        var logoutResponse = "It has not been iml";
        return Ok(logoutResponse);
    }
}