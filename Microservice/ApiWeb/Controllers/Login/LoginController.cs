using Microservice.ApiWeb.Dto.Credentials;
using Microservice.Application.Services.Authentication.Context;
using Microservice.Application.Services.Authentication.Interfaces;
using Microservice.Application.Services.Authorization;
using Microservice.Application.Services.Crud.Interfaces;
using Microservice.Application.Services.Security.Interfaces;
using Microservice.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.ApiWeb.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IHashProviderService _hashProviderService;
        private readonly ICrudService<User> _userCrudService;
        private readonly ITokenProviderService<SignedJwt> _tokenProviderService;

        public LoginController(ITokenProviderService<SignedJwt> TokenProviderService, IHashProviderService HashService, ICrudService<User> CrudService) { 
            this._tokenProviderService = TokenProviderService;
            this._hashProviderService  = HashService;
            this._userCrudService      = CrudService;
        }

        [HttpGet]
        public async Task<IActionResult> Login([FromBody] UserCredentialsDto UserCredentials) {
            //Credentials check
            if (UserCredentials          == null) { return BadRequest(); }
            if (UserCredentials.Email    == null) { return BadRequest(); }
            if (UserCredentials.Password == null) { return BadRequest(); }
            //Authenticate the User
            User? UserByEmail = await this._userCrudService.Retrieve(user => user.Email == UserCredentials.Email, user => user.UsersRoles);
            if (UserByEmail == null) { return BadRequest(); }
            //Password hash check
            if (!this._hashProviderService.Check(UserByEmail.Password!, UserCredentials.Password)) { return BadRequest(); }
            //Get the token string
            string JwtString = this._tokenProviderService.GetTokenString(UserByEmail);
            return Ok(JwtString);
        }

        [HttpGet("publicKey")]
        [Authorize(Policy=AuthorizationPolicies.USER, AuthenticationSchemes="AsymmetricSignedJwt")]
        public IActionResult GetPublicKey() {
            return Ok(this._tokenProviderService.GetPublicKey());
        }

        [HttpGet("claims")]
        [Authorize(Policy=AuthorizationPolicies.ADMIN, AuthenticationSchemes="AsymmetricSignedJwt")]
        public IActionResult GetTokenClaims() {
            //Access to request header 'Authorization: Bearer <TOKEN_STRING>'
            string TokenString = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if(!this._tokenProviderService.VerifyToken(TokenString)) { return BadRequest(); }
            return Ok(this._tokenProviderService.DecodeToken(TokenString));
        }
    }
}
