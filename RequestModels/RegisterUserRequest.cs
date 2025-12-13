namespace FinflowAPI.RequestModels;

public class RegisterUserRequest
{
    public string? firstName {get;set;}
    public string? lastName {get;set;}
    public string? userName {get;set;}
    public string? email {get;set;}
    public string? password {get;set;}
    public string? confirmPassword {get;set;}
    public string? phoneNumber {get;set;}
}