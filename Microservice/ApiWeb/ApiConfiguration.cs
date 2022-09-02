using Microservice.ApiWeb.Filters;
using System.Text.Json.Serialization;

namespace Microservice.ApiWeb
{
    public static class ApiConfiguration
    {
        public static IServiceCollection AddApiWeb(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            //Configure CORS Policies
            services.AddCors(options => { options.AddPolicy("ALL", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()); });
            //Configure Controllers with Global Filters and Json Options settings
            services.AddControllers(opts => opts.Filters.Add(new ExceptionInterceptorFilter()))
                    .AddJsonOptions(opts => opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); 
            return services;
        }
    }
}