using Microsoft.AspNetCore.Authorization;

namespace RoleBaseAuthorization.Attributes
{
    public class RequireFunctionAttribute : AuthorizeAttribute
    {
        public RequireFunctionAttribute(string functionCode)
        {
            Policy = $"Function_{functionCode}";
        }
    }

    public class RequirePrivilegeAttribute : AuthorizeAttribute
    {
        public RequirePrivilegeAttribute(string privilegeCode)
        {
            Policy = $"Privilege_{privilegeCode}";
        }
    }
}