using AutoMapper;
using Microservice.ApiWeb.Dto.Filters;
using Microservice.ApiWeb.Dto.UserResources.Response;
using Microservice.Application.Services.Crud.Interfaces;
using Microservice.Application.Services.Linq.Interfaces;
using Microservice.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Microservice.ApiWeb.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSearchController : ControllerBase
    {
        private readonly ICrudService<User> _userCrudService;
        private readonly ILinqBuilderService _linqBuilder;
        private readonly IMapper _mapper;

        public UserSearchController(ICrudService<User> CrudService, ILinqBuilderService LinqBuilder,IMapper Mapper)
        {
            this._userCrudService = CrudService;
            this._linqBuilder = LinqBuilder;
            this._mapper = Mapper;
        }

        [HttpGet] //Example: UserName Like Search ===>  Pattern="T", Action="StartsWith", StringFieldName="UserName"
        public async Task<IActionResult> GetUsersByStringFieldSearch([FromQuery]FilterDtoStringPredicate Filter)
        {
            if (Filter == null) { return BadRequest(); }
            Expression<Func<User, bool>> FilterExpr = this._linqBuilder.StringPredicate<User>(Filter.StringFieldName!,Filter.Action!,Filter.Pattern!);
            IEnumerable<User> Users = await this._userCrudService.RetrieveAll(FilterExpr);
            return Ok(this._mapper.Map<IEnumerable<User>,IEnumerable<UserDtoGetResponse>>(Users));
        }
    }
}
