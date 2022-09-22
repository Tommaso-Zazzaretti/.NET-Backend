using Microservice;
using Microservice.WebServer;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureWebServer();
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services, builder.Environment);
var app = builder.Build();
startup.Configure(app);