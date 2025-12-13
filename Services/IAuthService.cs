using System.Net;
using Microsoft.AspNetCore.Identity.Data;

namespace FinflowAPI.Services;

public interface IAuthService
{
    Task<(HttpStatusCode,object)> Login(LoginRequest request);
}