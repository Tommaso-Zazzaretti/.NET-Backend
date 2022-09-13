using Microservice.ApiWeb.Filters;
using Microservice.Application.Services.Security.Context;
using Microservice.Application.Services.Security.Interfaces;
using Microsoft.IdentityModel.Tokens;
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
            //Configure Controllers with Global Filters and Json Options de/serialization settings
            services.AddControllers(opts => opts.Filters.Add(new ExceptionInterceptorFilter()))
                    .AddJsonOptions(opts => opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            //Configure Authentication Schemas for API
            services.AddAuthentication().AddJwtBearer("AsymmetricSignedJwtSchema", opts => {
                ITokenProviderService<JWS> JwtService = services.BuildServiceProvider().GetRequiredService<ITokenProviderService<JWS>>();
                //Set the rules for accepting or rejecting a signed JWT. In an asymmetric signed scenario, the key used to verify the token signature is the public key
                opts.TokenValidationParameters = new TokenValidationParameters() {
                    ValidateIssuerSigningKey   = true,
                    IssuerSigningKey           = JwtService.GetSignatureVerificationKey(), //Public Key
                    ValidateIssuer             = true,
                    ValidIssuer                = JwtService.GetIssuer(),
                    ValidateAudience           = true,
                    ValidAudience              = JwtService.GetAudience(),
                    ValidateLifetime           = true,
                    ClockSkew                  = TimeSpan.Zero,
                    RequireSignedTokens        = true,
                    RequireExpirationTime      = true
                };
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