using Microservice.ApiWeb.Filters;
using Microsoft.OpenApi.Models;
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

            //Dev-Only configurations
            if (env.IsDevelopment()) { 
                //Registry a SwaggerGenerator to generate a json file according to the specifications of the openapi.json standard
                services.AddSwaggerGen( opts => { opts.SwaggerDoc("APIdocs", new OpenApiInfo { Title = "API Docs", Version = "v1" }); });
            }
            return services;
        }
    }
}