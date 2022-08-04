using Microservice.Infrastructure;

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
        }

        //App Launch Configuration
        public void Configure(WebApplication app) {
            IWebHostEnvironment Env = app.Environment;
            if (!Env.IsDevelopment()) {
            }
            app.Run();
        }
    }
}
