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
        public void ConfigureServices(IServiceCollection Services, IWebHostEnvironment Env) {
            Services.AddInfrastructure(this._configuration);
            Services.AddApplication();
            Services.AddApiWeb(this._configuration,Env);
        }

        //App Launch Configuration
        public void Configure(WebApplication app) {
            app.UseHsts();             //Force clients to use HTTPS (except for the first time)
            app.UseHttpsRedirection(); //Enable middleware to redirect all calls from http to https
            app.UseRouting();          //Enable route matching middleware to map requests to configured endpoints
            app.UseCors("ALL");        //Enable CORS protocol middlware using a policy configured in the ApiConfiguration.cs            
            app.UseAuthentication();   //Enable Authentication middleware
            app.UseAuthorization();    //Enable Authorization middleware 
            app.UseEndpoints(conf => { //Enable Endpoints calls
                conf.MapControllers(); 
            }); 
            //Dev-Only env endpoints middlewares
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger(); //Start up of the swagger endpoints to access openapi.json generated files  => http://<HOST>:<PORT>/swagger/<DOC_NAME>/swagger.json" 
                app.UseSwaggerUI(opts => {opts.SwaggerEndpoint("/swagger/APIdocs/swagger.json", "API Docs"); opts.RoutePrefix = string.Empty; }); //Start up of a User Interface html endpoint associated with a swagger docs => http://<HOST>:<PORT>/index.html
            }
            app.Run();
        }
    }
}
