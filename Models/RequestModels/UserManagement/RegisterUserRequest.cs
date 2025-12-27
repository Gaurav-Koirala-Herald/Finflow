namespace FinflowAPI.Models.RequestModels.UserManagement;

public class RegisterUserRequest : CommonApiRequest
{
   public string? email {get;set;}
   public string? passwordHash{get;set;}
   public string? mobileNumber{get;set;}
   public string? userName{get;set;}
   public string? fullName{get;set;}
   public long dob{get;set;}
   public string? gender{get;set;}
   public string? profilePicUrl{get;set;}
   public string? preferredLanguage{get;set;}
}
