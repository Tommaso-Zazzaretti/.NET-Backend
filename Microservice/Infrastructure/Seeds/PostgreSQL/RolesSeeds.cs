using Microservice.Domain.Models;

namespace Microservice.Infrastructure.Seeds.PostgreSQL
{
    public static class RolesSeeds
    {
        public static readonly IEnumerable<Role> Seeds = new List<Role> {
            new Role(){ RoleName="USER" },
            new Role(){ RoleName="ADMIN" },
            new Role(){ RoleName="SUPER-ADMIN" }
        };
    }
}
