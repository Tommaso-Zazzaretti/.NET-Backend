using Microservice.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Microservice.Application.Services.Authorization.Requirements
{
    public class AdminRequirements : IAuthorizationRequirement
    {
        private readonly ISet<string> _requiredRoles;

        public AdminRequirements() {
            this._requiredRoles = new HashSet<string>() {
                Roles.ADMIN
            };
        }

        public ISet<string> GetRequiredRoles() {
            return this._requiredRoles;
        }
    }
}
