using Microservice.Application.Services.Crud.Interfaces;
using Microservice.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.ApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICrudService<User> _service;

        public UserController(ICrudService<User> CrudService) {
            this._service = CrudService;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserByUsername(string username) {
            User? UserByUsername = await this._service.Retrieve(user => user.UserName == username);
            return (UserByUsername==null) ? BadRequest() : Ok(UserByUsername);
        }
    }
}
