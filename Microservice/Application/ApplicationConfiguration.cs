using Microservice.Application.Services.Crud;
using Microservice.Application.Services.Crud.Interfaces;
using Microservice.Application.Services.Download;
using Microservice.Application.Services.Download.Interfaces;
using Microservice.Application.Services.Linq;
using Microservice.Application.Services.Linq.Interfaces;
using Microservice.Application.Services.Security;
using Microservice.Application.Services.Security.Context;
using Microservice.Application.Services.Security.Interfaces;
using Microservice.Application.Services.Upload;
using Microservice.Application.Services.Upload.Contexts;
using Microservice.Application.Services.Upload.Interfaces;
using Microservice.Domain.Models;
using Microservice.Infrastructure.DatabaseContexts;
using System.Reflection;

namespace Microservice.Application
{
    public static class ApplicationConfiguration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //Registry all Dto Profilers
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //Registry all Linq Services 
            services.AddSingleton<ILinqGeneratorService>(new LinqGeneratorService(Assembly.GetExecutingAssembly(),"Microservice.Domain.Models"));
            services.AddSingleton<ILinqCombinatorService, LinqCombinatorService>();
            //Registry all PostgreSQL Crud Services
            services.AddScoped<ICrudService<User>      , CrudService<User,DbContextPostgreSql>>();
            services.AddScoped<ICrudService<Role>      , CrudService<Role,DbContextPostgreSql>>();
            services.AddScoped<ICrudService<UsersRoles>, CrudService<UsersRoles, DbContextPostgreSql>>();
            //Hash Encryption Helper Service for passwords & sensitive data
            services.AddSingleton<IHashProviderService, HashProviderService>();
            //Registry Security Token Provider
            services.AddScoped<ITokenProviderService<SignedJwt>, TokenProviderService>();
            //Registry an Http Request Accessor for Upload Servixw
            services.AddHttpContextAccessor();
            //Registry all Upload Services
            services.AddSingleton<IUploadService<MultipartFormData>, UploadMultipartFormDataService>();
            //Registry all Download Services
            services.AddSingleton<IDownloadService, DownloadService>();
            return services;
        }
    }
}
