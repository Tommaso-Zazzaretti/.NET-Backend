using Microservice.Application.Services.Authentication.Context.Base;

namespace Microservice.Application.Services.Authentication.Context
{
    //POCO Class used for DI! Multiple implementations of ITokenProviderService
    public class SignedJwt : TokenTypeContext // => Asymmetrically Signed JWT, but not asymmetrically encrypted
    {
    }
}
