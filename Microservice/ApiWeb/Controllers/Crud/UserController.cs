using AutoMapper;
using Microservice.ApiWeb.Dto.UserResources.Request;
using Microservice.ApiWeb.Dto.UserResources.Response;
using Microservice.Application.Services.Crud.Interfaces;
using Microservice.Application.Services.Security.Interfaces;
using Microservice.Domain.Constants;
using Microservice.Domain.Models;
using Microservice.Infrastructure.DatabaseContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Microservice.ApiWeb.Controllers.Crud
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DbContextPostgreSql _dbCtx; // Only used to open transactions
        private readonly ICrudService<User> _userCrudService;
        private readonly ICrudService<UsersRoles> _usersRolesCrudService;
        private readonly IHashProviderService _encryptorService;
        private readonly IMapper _mapper;

        public UserController(DbContextPostgreSql Ctx, ICrudService<User> CrudService, ICrudService<UsersRoles> UsersRolesCrudService, IHashProviderService EncryptorService, IMapper Mapper) {
            this._dbCtx = Ctx;
            this._userCrudService = CrudService;
            this._usersRolesCrudService = UsersRolesCrudService;
            this._encryptorService = EncryptorService;
            this._mapper = Mapper;
        }

        [HttpGet("{Username}")]
        public async Task<IActionResult> GetUserByUsername(string Username) {
            if (Username == null) { return BadRequest(); }
            User? UserByUsername = await this._userCrudService.Retrieve(user => user.UserName == Username);
            return (UserByUsername==null) ? BadRequest() : Ok(this._mapper.Map<UserDtoGetResponse>(UserByUsername));
        }

        [HttpPost]
        public async Task<IActionResult> InsertNewUser([FromBody] UserDtoCreateRequest UserDtoToAdd) {
            if (UserDtoToAdd==null || string.IsNullOrEmpty(UserDtoToAdd.Password)) { return BadRequest(); }
            User UserToAdd = this._mapper.Map<User>(UserDtoToAdd);
            UserToAdd.Password = this._encryptorService.Hash(UserToAdd.Password!);
            //Open Transaction ( [1] Insert new user, [2] Insert new UserRole )
            IDbContextTransaction Transaction = this._dbCtx.Database.BeginTransaction();
            User       AddedUser     = await this._userCrudService.Create(UserToAdd);
            UsersRoles AddedUserRole = await this._usersRolesCrudService.Create(new UsersRoles() { UserName = AddedUser.UserName, RoleName = Roles.USER });
            Transaction.Commit(); //Close Transaction
            return Ok(this._mapper.Map<UserDtoGetResponse>(AddedUser));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserData([FromBody] UserDtoUpdateDataRequest UserDtoToUpdate) {
            if (UserDtoToUpdate == null) { return BadRequest(); }
            Expression<Func<User, bool>> WhereClause = user => user.UserName == UserDtoToUpdate.UserName;
            Action<User> UpdateFunc = user => { user.Name = UserDtoToUpdate.Name; user.Surname = UserDtoToUpdate.Surname; };
            User UpdatedUser = await this._userCrudService.UpdatePartially(WhereClause,UpdateFunc);
            return Ok(this._mapper.Map<UserDtoGetResponse>(UpdatedUser));
        }

        [HttpPut("pwd")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UserDtoUpdatePasswordRequest UserDtoToUpdate)
        {
            if (UserDtoToUpdate == null || string.IsNullOrEmpty(UserDtoToUpdate.Email) || string.IsNullOrEmpty(UserDtoToUpdate.OldPassword) || string.IsNullOrEmpty(UserDtoToUpdate.NewPassword)) { return BadRequest(); }
            //Mail exist check
            User? UserByEmail = await this._userCrudService.Retrieve(user => user.Email == UserDtoToUpdate.Email);
            if (UserByEmail == null) { return BadRequest("Wrong Email"); }
            //Old Password match check
            if(!this._encryptorService.Check(UserByEmail.Password!, UserDtoToUpdate.OldPassword)) { return BadRequest("Wrong Pwd"); }
            //Set new Password
            Action<User> UpdatePwdFunc = user => { user.Password = this._encryptorService.Hash(UserDtoToUpdate.NewPassword); };
            User UpdatedUser = await this._userCrudService.UpdatePartially(UserByEmail, UpdatePwdFunc);
            return Ok(this._mapper.Map<UserDtoGetResponse>(UpdatedUser));
        }

        [HttpDelete("{Username}")]
        public async Task<IActionResult> DeleteUserByUsername(string Username) {
            if (Username == null) { return BadRequest(); }
            User? DeletedUser = await this._userCrudService.Delete(user => user.UserName == Username);
            return (DeletedUser == null) ? BadRequest() : Ok(this._mapper.Map<UserDtoGetResponse>(DeletedUser));
        }
    }
}