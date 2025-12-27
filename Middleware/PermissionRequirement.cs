using Microsoft.AspNetCore.Authorization;

namespace FinFlowAPI.Middleware
{
    public class FunctionRequirement : IAuthorizationRequirement
    {
        public string FunctionCode { get; }

        public FunctionRequirement(string functionCode)
        {
            FunctionCode = functionCode;
        }
    }

    public class PrivilegeRequirement : IAuthorizationRequirement
    {
        public string PrivilegeCode { get; }

        public PrivilegeRequirement(string privilegeCode)
        {
            PrivilegeCode = privilegeCode;
        }
    }

    public class FunctionHandler : AuthorizationHandler<FunctionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            FunctionRequirement requirement)
        {
            // SuperAdmin has all permissions
            if (context. User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var functions = context.User. Claims
                .Where(c => c.Type == "function")
                .Select(c => c.Value);

            if (functions.Contains(requirement.FunctionCode))
            {
                context.Succeed(requirement);
            }

            return Task. CompletedTask;
        }
    }

    public class PrivilegeHandler : AuthorizationHandler<PrivilegeRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PrivilegeRequirement requirement)
        {
            // SuperAdmin has all permissions
            if (context.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var privileges = context.User.Claims
                .Where(c => c.Type == "privilege")
                .Select(c => c.Value);

            if (privileges.Contains(requirement. PrivilegeCode))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}