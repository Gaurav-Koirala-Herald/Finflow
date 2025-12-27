using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace RoleBaseAuthorization.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        // marker
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // The required permission will be passed via resource (HttpContext) item "permission"
            if (context.Resource is Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext mvcContext)
            {
                if (mvcContext.HttpContext.Items.TryGetValue("permission", out var raw) && raw is string required)
                {
                    var has = context.User.Claims.Any(c => c.Type == "permission" && c.Value == required);
                    if (has) context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}