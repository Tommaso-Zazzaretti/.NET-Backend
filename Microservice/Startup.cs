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
        public void Configure(WebApplication App) {
            App.UseHsts();
            App.UseHttpsRedirection();
            App.UseRouting();   //Enable route matching middleware to map requests to configured endpoints
            App.UseCors("ALL"); //Enable CORS protocol middlware using a policy configured in the ApiConfiguration.cs            
            App.UseAuthentication(); //Enable Authentication middleware
            App.UseAuthorization();  //Enable Authorization middleware 
            App.UseEndpoints(endpoints => { endpoints.MapControllers(); }); //Enable middleware for endpoints calls
            //Dev-Only env endpoints middlewares
            if (App.Environment.IsDevelopment()) {
                //Start up of the swagger endpoints to access openapi.json generated files  => http://<HOST>:<PORT>/swagger/<DOC_NAME>/swagger.json" 
                App.UseSwagger();
                //Start up of a User Interface html endpoint associated with a swagger docs => http://<HOST>:<PORT>/index.html
                App.UseSwaggerUI(opts => {opts.SwaggerEndpoint("/swagger/APIdocs/swagger.json", "API Docs"); opts.RoutePrefix = string.Empty; }); 
            }
            App.Run();
        }
    }
}
