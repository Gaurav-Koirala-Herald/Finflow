using System.Net;
using Microsoft.AspNetCore.Identity.Data;

namespace FinflowAPI.Services;

public class AuthService : IAuthService
{
    public async Task<(HttpStatusCode, object)> Login(LoginRequest request)
    {
        throw new NotImplementedException();
    }
}