using Microservice.ApiWeb.Dto.Credentials;
using Microservice.Application.Services.Authentication.Context;
using Microservice.Application.Services.Authentication.Interfaces;
using Microservice.Application.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.ApiWeb.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenProviderService<SignedJwt> _tokenProviderService;

        public LoginController(ITokenProviderService<SignedJwt> TokenProviderService) { 
            this._tokenProviderService = TokenProviderService; 
        }

        [HttpGet]
        public async Task<IActionResult> Login([FromBody] UserCredentialsDto UserCredentials) {
            if (UserCredentials          == null) { return BadRequest(); }
            if (UserCredentials.Email    == null) { return BadRequest(); }
            if (UserCredentials.Password == null) { return BadRequest(); }
            string? JwtString = await this._tokenProviderService.GetTokenString(UserCredentials.Email, UserCredentials.Password);
            if(JwtString == null) { return BadRequest(); }
            return Ok(JwtString);
        }

        [HttpGet("test")]
        [Authorize(Policy=AuthorizationPolicies.ADMIN, AuthenticationSchemes="AsymmetricSignedJwt")]
        public IActionResult GetTokenClaims() {
            //Access to request header 'Authorization: Bearer <TOKEN_STRING>'
            string TokenString = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if(!this._tokenProviderService.VerifyToken(TokenString)) { return BadRequest(); }
            return Ok(this._tokenProviderService.DecodeToken(TokenString));
        }
    }
}
