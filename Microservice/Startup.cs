﻿using Microservice.Infrastructure;
using Microservice.Application;
using Microservice.ApiWeb;
namespace Microservice
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) {
            this._configuration = configuration;
        }

        //App Build Configuration
        public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env) {
            services.AddInfrastructure(this._configuration);
            services.AddApplication();
            services.AddApiWeb(this._configuration,env);
        }

        //App Launch Configuration
        public void Configure(WebApplication app) {
            if (app.Environment.IsDevelopment()) {
            }
            app.UseRouting();
            app.UseCors("ALL");
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.Run();
        }
    }
}
