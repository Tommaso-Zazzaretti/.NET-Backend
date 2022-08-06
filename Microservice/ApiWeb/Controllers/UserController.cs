using Microservice.Application.Services.Crud.Interfaces;
using Microservice.Application.Services.Security.Interfaces;
using Microservice.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.ApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICrudService<User> _crudService
            ;
        private readonly IHashProviderService _pwdEncryptorService;

        public UserController(ICrudService<User> CrudService, IHashProviderService PwdEncryptorService) {
            this._crudService = CrudService;
            this._pwdEncryptorService = PwdEncryptorService;
        }

        [HttpGet("{Username}")]
        public async Task<IActionResult> GetUserByUsername(string Username) {
            if (Username == null) { return BadRequest(); }
            User? UserByUsername = await this._crudService.Retrieve(user => user.UserName == Username,user => user.UsersRoles);
            return (UserByUsername==null) ? BadRequest() : Ok(UserByUsername);
        }

        [HttpPost]
        public async Task<IActionResult> InsertNewUser([FromBody]User UserToAdd)
        {
            if (UserToAdd==null || string.IsNullOrEmpty(UserToAdd.Password)) { return BadRequest(); }
            UserToAdd.Password = this._pwdEncryptorService.Hash(UserToAdd.Password);
            User AddedUser = await this._crudService.Create(UserToAdd);
            return Ok(AddedUser);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody]User UserToUpdate)
        {
            if (UserToUpdate == null || string.IsNullOrEmpty(UserToUpdate.Password)) { return BadRequest(); }
            UserToUpdate.Password = this._pwdEncryptorService.Hash(UserToUpdate.Password);
            User UpdatedUser = await this._crudService.Update(UserToUpdate);
            return Ok(UpdatedUser);
        }

        [HttpDelete("{Username}")]
        public async Task<IActionResult> DeleteUserByUsername(string Username)
        {
            if (Username == null) { return BadRequest(); }
            User? DeletedUser = await this._crudService.Delete(user => user.UserName == Username);
            return (DeletedUser == null) ? BadRequest() : Ok(DeletedUser);
        }
    }
}