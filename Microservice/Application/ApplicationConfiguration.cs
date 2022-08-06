using Microservice.Application.Services.Crud;
using Microservice.Application.Services.Crud.Interfaces;
using Microservice.Application.Services.Security;
using Microservice.Application.Services.Security.Interfaces;
using Microservice.Domain.Models;
using Microservice.Infrastructure.DatabaseContexts;

namespace Microservice.Application
{
    public static class ApplicationConfiguration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //Registry all Crud Services
            services.AddScoped<ICrudService<User>, CrudService<User,DbContextPostgreSql>>();
            services.AddScoped<ICrudService<Role>, CrudService<Role, DbContextPostgreSql>>();
            //Encrypt Helper Service
            services.AddSingleton<IHashProviderService, HashProviderService>();
            return services;
        }
    }
}
