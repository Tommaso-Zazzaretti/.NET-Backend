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
        public void Configure(WebApplication App) {
            //Enable route matching middleware to map requests to configured endpoints
            App.UseRouting();
            //Enable CORS protocol middlware using a policy configured in the ApiConfiguration.cs
            App.UseCors("ALL");
            //Enable Authentication middleware
            App.UseAuthentication();
            //Enable Authorization middleware 
            App.UseAuthorization();
            //Enable middleware for endpoints calls
            App.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            //Dev-Only env endpoints middlewares
            if (App.Environment.IsDevelopment()) {
                //Start up of the swagger endpoints to access openapi.json generated files  => http://<HOST>:<PORT>/swagger/<DOC_NAME>/swagger.json" 
                App.UseSwagger();
                //Start up of a User Interface html endpoint associated with a swagger docs => http://<HOST>:<PORT>/index.html
                App.UseSwaggerUI(opts => { opts.SwaggerEndpoint("/swagger/APIdocs/swagger.json", "API Docs"); opts.RoutePrefix = string.Empty; });
            }
            App.Run();
        }
    }
}
