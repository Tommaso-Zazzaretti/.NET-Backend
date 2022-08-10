using Microservice.Domain.Constants;
using Microservice.Domain.Models;

namespace Microservice.Infrastructure.Seeds.PostgreSQL
{
    public static class RolesSeeds
    {
        public static readonly IEnumerable<Role> Seeds = new List<Role> {
            new Role(){ RoleName = Roles.USER },
            new Role(){ RoleName = Roles.ADMIN },
            new Role(){ RoleName = Roles.SUPERADMIN }
        };
    }
}
