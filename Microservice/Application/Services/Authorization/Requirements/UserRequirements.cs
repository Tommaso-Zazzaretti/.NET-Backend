using Microservice.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Microservice.Application.Services.Authorization.Requirements
{
    public class UserRequirements : IAuthorizationRequirement
    {
        private readonly ISet<string> _requiredRoles;

        public UserRequirements() { 
            this._requiredRoles = new HashSet<string>() { 
                Roles.USER 
            };
        }

        public ISet<string> GetRequiredRoles() { 
            return this._requiredRoles; 
        }
    }
}
