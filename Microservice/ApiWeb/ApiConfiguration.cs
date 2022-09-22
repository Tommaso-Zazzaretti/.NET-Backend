using Microservice.ApiWeb.Filters;
using Microservice.Application.Services.Authentication.Context;
using Microservice.Application.Services.Authentication.Interfaces;
using Microservice.Application.Services.Authorization;
using Microservice.Application.Services.Authorization.Requirements;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace Microservice.ApiWeb
{
    public static class ApiConfiguration
    {
        public static IServiceCollection AddApiWeb(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            //Configure HTTP Strict-Transport-Security Header
            services.AddHsts(options => {
                options.Preload = true;                 //Make the server available only via Https after first call
                options.IncludeSubDomains = true;       //Extend HSTS rule to subdomains
                options.MaxAge = TimeSpan.FromDays(60); //Time for which clients must remember that the site can only be reached via https
            });
            //Configure HTTPS Redirection
            services.AddHttpsRedirection(opts => {
                opts.RedirectStatusCode = 307; //TemporaryRedirect
                opts.HttpsPort = int.Parse(configuration["Kestrel:Endpoints:Https:Url"].Split(':').Last()); //Redirect port
            });
            //Configure CORS Policies
            services.AddCors(options => { 
                options.AddPolicy("ALL", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()); 
            });
            //Configure Controllers with Global Filters and Json Options de/serialization settings
            services.AddControllers(opts => opts.Filters.Add(new ExceptionInterceptorFilter()))
                    .AddJsonOptions(opts => opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            //Configure Authentication Schemas for API
            services.AddAuthentication().AddJwtBearer("AsymmetricSignedJwt", opts => {
                //Set the rules for accepting or rejecting a signed JWT
                ITokenProviderService<SignedJwt> JwtService = services.BuildServiceProvider().GetRequiredService<ITokenProviderService<SignedJwt>>();
                opts.TokenValidationParameters = JwtService.GetTokenValidationParameters();
            });
            //Configure Authorization Policies
            services.AddAuthorization(opts => {
                opts.AddPolicy(AuthorizationPolicies.USER       , policy => policy.AddRequirements(new UserRequirements()));
                opts.AddPolicy(AuthorizationPolicies.ADMIN      , policy => policy.AddRequirements(new AdminRequirements()));
                opts.AddPolicy(AuthorizationPolicies.SUPER_ADMIN, policy => policy.AddRequirements(new SuperAdminRequirements()));
            });
            
            //Dev-Only configurations
            if (env.IsDevelopment()) { 
                //Registry a SwaggerGenerator to generate a json file according to the specifications of the openapi.json standard
                services.AddSwaggerGen( opts => { opts.SwaggerDoc("APIdocs", new OpenApiInfo { Title = "API Docs", Version = "v1" }); });
            }
            return services;
        }
    }
}