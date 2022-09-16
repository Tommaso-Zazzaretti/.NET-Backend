using Microservice.Application.Services.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Microservice.Application.Services.Authorization.Handlers
{
    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirements>
    {
        //Write the logic to verify that an authenticated user meets to perform a certain action (CanRead action)
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirements requirement)
        {
            //Role Claim exist check:
            Claim? RoleClaim = context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
            if (RoleClaim == null) {
                context.Fail();
                return Task.CompletedTask;
            }
            //Role Claim Requirement value check:
            ISet<string> RolesSet = new HashSet<string>();
            RoleClaim.Value.Split(',').ToList().ForEach(role => RolesSet.Add(role));
            if (RolesSet.IsSupersetOf(requirement.GetRequiredRoles())) {
                context.Succeed(requirement); //Requirements met, authorization granted
            } else {
                context.Fail(); //Requirements not met, authorization not granted
            }
            return Task.CompletedTask;
        }
    }
}
