namespace Microservice.Domain.Models
{
    //PostgreSQL roles Table
    public class Role
    {
        public string? RoleName { get; set; }

        // Navigation properties
        public virtual ICollection<UsersRoles>? UsersRoles { get; private set; }
    }
}
