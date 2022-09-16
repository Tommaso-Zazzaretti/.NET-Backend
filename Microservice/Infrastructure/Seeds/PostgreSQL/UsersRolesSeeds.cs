using Microservice.Domain.Constants;
using Microservice.Domain.Models;

namespace Microservice.Infrastructure.Seeds.PostgreSQL
{
    public static class UsersRolesSeeds
    {
        public static readonly IEnumerable<UsersRoles> Seeds = new List<UsersRoles> {
            new UsersRoles(){ UserName="Tom96", RoleName=Roles.USER },
            new UsersRoles(){ UserName="Tom96", RoleName=Roles.ADMIN },
            new UsersRoles(){ UserName="Tom96", RoleName=Roles.SUPERADMIN },
            new UsersRoles(){ UserName="UserX", RoleName=Roles.USER },
            new UsersRoles(){ UserName="AdminX", RoleName=Roles.ADMIN }
        };
    }
}
