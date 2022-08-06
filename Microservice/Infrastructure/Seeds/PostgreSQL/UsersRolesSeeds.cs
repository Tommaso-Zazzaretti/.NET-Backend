using Microservice.Domain.Models;

namespace Microservice.Infrastructure.Seeds.PostgreSQL
{
    public static class UsersRolesSeeds
    {
        public static readonly IEnumerable<UsersRoles> Seeds = new List<UsersRoles> {
            new UsersRoles(){ UserName="Tom96", RoleName="USER" },
            new UsersRoles(){ UserName="Tom96", RoleName="ADMIN" },
            new UsersRoles(){ UserName="Tom96", RoleName="SUPER-ADMIN" }
        };
    }
}
