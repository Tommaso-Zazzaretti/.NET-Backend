using Microservice.Application.Services.Security.Context.Base;
using Microservice.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Microservice.Application.Services.Security.Interfaces
{
    public interface ITokenProviderService<T> where T : TokenTypeContext
    {
        //Token Methods
        public Task<string?> GetTokenString(string Email, string Password);
        public IDictionary<string, string> DecodeToken(string TokenString);
        public bool VerifyToken(string TokenString);

        //PublicKey API call method
        public RsaSecurityKey GetSignatureVerificationKey();

        //Utilities for D.I. AddAuthentication Schema
        public TokenValidationParameters GetTokenValidationParameters();
    }
}
