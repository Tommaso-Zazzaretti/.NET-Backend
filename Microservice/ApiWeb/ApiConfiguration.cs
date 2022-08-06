using System.Text.Json.Serialization;

namespace Microservice.ApiWeb
{
    public static class ApiConfiguration
    {
        public static IServiceCollection AddApiWeb(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddCors(options => { options.AddPolicy("ALL", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()); });
            services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); 
            return services;
        }
    }
}