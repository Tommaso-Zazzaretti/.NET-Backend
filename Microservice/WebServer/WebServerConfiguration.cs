using Microservice.WebServer.Ssl;
using System.Security.Cryptography.X509Certificates;

namespace Microservice.WebServer
{
    public static class WebServerConfiguration
    {
        public static WebApplicationBuilder ConfigureWebServer(this WebApplicationBuilder builder)
        {
            /* Developement Environment Only:
            Use a TLS SelfSigned X509 certificate to enable Https on localhost for the dev/test environment.
            But it's not enough: modern versions of browsers rejected selfSigned certificates. 
            For Google Chrome, type 'chrome://flags/#allow-insecure-localhost' and enable it- */
            if (builder.Environment.IsDevelopment()) {  
                builder.WebHost.ConfigureKestrel(serverOptions => {
                    X509Certificate2? Certificate = HttpsDevHelper.InstallSelfSignedLocalhostX509Certificate(StoreName.Root);
                    if (Certificate != null) {
                        serverOptions.ConfigureHttpsDefaults(httpsOpts => { 
                            httpsOpts.ServerCertificate = Certificate; 
                        });
                    }
                });
            }
            return builder;
        }
    }
}
