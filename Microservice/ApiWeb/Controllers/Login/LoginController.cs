using Microservice.ApiWeb.Dto.Credentials;
using Microservice.Application.Services.Security.Context;
using Microservice.Application.Services.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.ApiWeb.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenProviderService<JWS> _tokenProviderService;

        public LoginController(ITokenProviderService<JWS> TokenProviderService) { 
            this._tokenProviderService = TokenProviderService; 
        }

        [HttpGet]
        public async Task<IActionResult> Login([FromBody] UserCredentialsDto UserCredentials)
        {
            if (UserCredentials          == null) { return BadRequest(); }
            if (UserCredentials.Email    == null) { return BadRequest(); }
            if (UserCredentials.Password == null) { return BadRequest(); }
            string? JwtString = await this._tokenProviderService.GetJwtAsString(UserCredentials.Email, UserCredentials.Password);
            if(JwtString == null) { return BadRequest(); }
            return Ok(JwtString);
        }

        [HttpGet("test")]
        [Authorize(AuthenticationSchemes = "AsymmetricSignedJwtSchema")]
        public IActionResult DecodeToken() {
            //Access to request header 'Authorization: Bearer <TOKEN_STRING>'
            string TokenString = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            return Ok(this._tokenProviderService.DecodeToken(TokenString));
        }
    }
}
