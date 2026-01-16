using ToDo.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace ToDo.Authorization.Handlers
{
    public class SameUsernameAuthorizationHandler:AuthorizationHandler<SameUsernameRequirement>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameUsernameRequirement requirement)
        {
            string? requestedUsername = null;

            if (context.Resource is HttpContext httpContext)
            {
                requestedUsername = httpContext.GetRouteValue("username")?.ToString()
                 ?? httpContext.Request.Query["username"].ToString();
            }
            if (string.IsNullOrWhiteSpace(requestedUsername))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            var tokenUsername = context.User.FindFirst(ClaimTypes.Name)?
                                     .Value
                                 ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)?
                                     .Value
                                 ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?
                                     .Value;

            if (string.IsNullOrWhiteSpace(tokenUsername))
            {
                return Task.CompletedTask;
            }
            if (string.Equals(tokenUsername, requestedUsername, StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
