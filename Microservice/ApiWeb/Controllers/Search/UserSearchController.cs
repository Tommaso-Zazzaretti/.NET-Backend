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
        private readonly ILinqGeneratorService  _linqBuilder;
        private readonly ILinqCombinatorService _linqCombiner;
        private readonly IMapper _mapper;

        public UserSearchController(ICrudService<User> CrudService, ILinqGeneratorService LinqBuilder, ILinqCombinatorService LinqCombiner, IMapper Mapper) {
            this._userCrudService = CrudService;
            this._linqBuilder  = LinqBuilder;
            this._linqCombiner = LinqCombiner;
            this._mapper = Mapper;
        }

        [HttpGet("stringpred")] //Example: UserName Like Search ===>  Pattern="To", Action="StartsWith", StringFieldName="UserName"
        public async Task<IActionResult> GetUsersByStringFieldSearch([FromQuery]StringPatternPredicateFilterDto Filter)
        {
            if (Filter == null) { return BadRequest(); }
            // Convert { "UserName","StartsWith","To"  }    TO LINQ     user => user.UserName.StartsWith("To")
            Expression<Func<User, bool>> FilterExpr = this._linqBuilder.StringPredicate<User>(Filter.StringFieldName!,Filter.Action!,Filter.Pattern!);
            IEnumerable<User> Users = await this._userCrudService.RetrieveAll(FilterExpr);
            return Ok(this._mapper.Map<IEnumerable<User>,IEnumerable<UserDtoGetResponse>>(Users));
        }

        [HttpGet("multiplestringpred")] 
        public async Task<IActionResult> GetUsersByMultipleStringFieldsSearch([FromBody] StringPatternPredicateFilterDto[] Filters)
        {
            if(!Filters.Any()) { return BadRequest(); }
            IEnumerable<Expression<Func<User, bool>>> FiltersExpr = Filters.Select(f => this._linqBuilder.StringPredicate<User>(f.StringFieldName!, f.Action!, f.Pattern!));
            if(FiltersExpr.Count()==1) {
                IEnumerable<User> UsersBySinglePredicate = await this._userCrudService.RetrieveAll(FiltersExpr.Single());
                return Ok(this._mapper.Map<IEnumerable<User>, IEnumerable<UserDtoGetResponse>>(UsersBySinglePredicate));
            }
            IEnumerator<Expression<Func<User, bool>>> Iterator = FiltersExpr.GetEnumerator();
            Iterator.MoveNext(); //Move to first expression
            Expression<Func<User, bool>>? FilterExpr = Iterator.Current;
            while (Iterator.MoveNext()) {
                FilterExpr = this._linqCombiner.And<User>(FilterExpr,Iterator.Current);
            }
            if(FilterExpr == null) { return BadRequest(); }
            IEnumerable<User> UsersByMultiplePredicate = await this._userCrudService.RetrieveAll(FilterExpr);
            return Ok(this._mapper.Map<IEnumerable<User>, IEnumerable<UserDtoGetResponse>>(UsersByMultiplePredicate));
        }

        [HttpGet("equality")] //Example: UserName filter ===>  EqualsConst="Tommaso", StringFieldName="UserName"
        public async Task<IActionResult> GetUsersByStringFieldEqualitySearch([FromQuery] EqualityFilterDto<string> Filter)
        {
            if (Filter == null) { return BadRequest(); }
            Expression<Func<User, bool>> FilterExpr = this._linqBuilder.EqualityPredicate<User, string>(Filter.StringFieldName!, Filter.EqualsConst!);
            IEnumerable<User> Users = await this._userCrudService.RetrieveAll(FilterExpr);
            return Ok(this._mapper.Map<IEnumerable<User>, IEnumerable<UserDtoGetResponse>>(Users));
        }
    }
}
