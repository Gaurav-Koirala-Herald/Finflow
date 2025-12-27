using System.Net;

namespace FinflowAPI.Models.ResponseModels.UserManagement;

public class RegisterUserResponse
{
    public string? message {get;set;}
    public HttpStatusCode responseCode {get;set;}
}