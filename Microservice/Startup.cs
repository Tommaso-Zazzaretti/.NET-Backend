using Microservice.Infrastructure;
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
            app.UseRouting();
            //Start up of CORS protocol middlware using a policy configured in the ApiConfiguration.cs
            app.UseCors("ALL");
            //Start up of the endpoints associated with the controllers
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            //Dev-Only env endpoints middlewares
            if (app.Environment.IsDevelopment()) {
                //Start up of the swagger endpoints to access openapi.json generated files  => http://<HOST>:<PORT>/swagger/<DOC_NAME>/swagger.json" 
                app.UseSwagger();
                //Start up of a User Interface html endpoint associated with a swagger docs => http://<HOST>:<PORT>/index.html
                app.UseSwaggerUI(opts => { opts.SwaggerEndpoint("/swagger/APIdocs/swagger.json", "API Docs"); opts.RoutePrefix = string.Empty; });
            }
            app.Run();
        }
    }
}
