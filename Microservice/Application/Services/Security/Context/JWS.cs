using Microservice.Application.Services.Security.Context.Base;

namespace Microservice.Application.Services.Security.Context
{
    //POCO Class used for DI! Multiple implementations of ITokenProviderService
    public class JWS : TokenTypeContext // => Asymmetrically Signed JWT, but not asymmetrically encrypted
    {
    }
}
