using Microservice.Application.Services.Security;
using Microservice.Application.Services.Security.Interfaces;
using Microservice.Domain.Models;

namespace Microservice.Infrastructure.Seeds.PostgreSQL
{
    public static class UsersSeeds
    {
        private static readonly IHashProviderService _service = new HashProviderService(32,16,10000);
        public static readonly IEnumerable<User> Seeds = new List<User> {
            new User(){
                UserName    = "Tom96",
                Name        = "Tommaso",
                Surname     = "Zazzaretti",
                Email       = "tommaso_zazzaretti_96@gmail.com",
                Password    = _service.Hash("P@ssw0rd")
            }
        };
    }
}