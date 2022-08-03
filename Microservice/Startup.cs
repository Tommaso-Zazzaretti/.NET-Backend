namespace Microservice
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) {
            this._configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env) {
        }

        public void Configure(WebApplication app, IWebHostEnvironment env) {
            if (!app.Environment.IsDevelopment()) {
            }
            app.Run();
        }
    }
}
