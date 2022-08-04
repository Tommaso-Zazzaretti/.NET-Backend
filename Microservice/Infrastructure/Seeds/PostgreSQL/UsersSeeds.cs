using Microservice.Domain.Models;

namespace Microservice.Infrastructure.Seeds.PostgreSQL
{
    public static class UsersSeeds
    {
        public static readonly IEnumerable<User> Seeds = new List<User> {
            new User(){ 
                UserName    = "Tom96",
                Name        = "Tommaso",
                Surname     = "Zazzaretti",
                Email       = "tommaso.zazzaretti96@gmail.com",
                Password    = "P@ssw0rd" 
            }
        };
    }
}