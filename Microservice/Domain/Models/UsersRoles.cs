namespace Microservice.Domain.Models
{
    public class UsersRoles
    {
        public string? UserName { get; set; }
        public string? RoleName { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Role? Role { get; set; }
    }
}
