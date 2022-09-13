using Microservice.Application.Services.Security.Context.Base;
using Microservice.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Microservice.Application.Services.Security.Interfaces
{
    public interface ITokenProviderService<T> where T : TokenTypeContext
    {
        public Task<string?>  GetJwtAsString(string Email, string Password);
        public IDictionary<string,string> DecodeToken(string TokenString);
        public RsaSecurityKey GetSignatureVerificationKey();


        //Utilities for D.I. (Token Validation Parameters)
        public string GetIssuer();
        public string GetAudience();

    }
}
