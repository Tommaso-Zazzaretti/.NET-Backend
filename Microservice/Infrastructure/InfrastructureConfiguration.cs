using Microservice.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Infrastructure
{
    public static class InfrastructureConfiguration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //IDENTITY DB CONFIGURATION
            string PostgresConnectionString = configuration["ConnectionStrings:PostgreSqlIdentityDB"];
            services.AddDbContext<DbContextPostgreSql>(opts => {
                opts.UseNpgsql(PostgresConnectionString)/*.LogTo(Console.WriteLine)*/.EnableSensitiveDataLogging();
            });
            return services;
        }
    }
}
