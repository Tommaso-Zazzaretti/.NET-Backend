using Microservice.Application.Services.Authentication.Context.Base;
using Microservice.Domain.Models;
using Microsoft.IdentityModel.Tokens;

namespace Microservice.Application.Services.Authentication.Interfaces
{
    public interface ITokenProviderService<T> where T : TokenTypeContext
    {
        //Token Methods
        public string GetTokenString(User AuthenticatedUser);
        public IDictionary<string, string> DecodeToken(string TokenString);
        public bool VerifyToken(string TokenString);

        //PublicKey API call method
        public string GetPublicKey();

        //Utilities for D.I. AddAuthentication Schema
        public TokenValidationParameters GetTokenValidationParameters();
    }
}
