using Microservice.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Microservice.Application.Services.Authorization.Requirements
{
    public class SuperAdminRequirements : IAuthorizationRequirement
    {
        private readonly ISet<string> _requiredRoles;

        public SuperAdminRequirements() {
            this._requiredRoles = new HashSet<string>() {
                Roles.SUPERADMIN
            };
        }

        public ISet<string> GetRequiredRoles() {
            return this._requiredRoles;
        }
    }
}