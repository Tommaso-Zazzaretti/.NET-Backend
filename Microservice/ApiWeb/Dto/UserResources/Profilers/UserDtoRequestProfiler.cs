using AutoMapper;
using Microservice.ApiWeb.Dto.UserResources.Request;
using Microservice.Domain.Models;

namespace Microservice.ApiWeb.Dto.UserResources.Profilers
{
    public class UserDtoRequestProfiler : Profile
    {
        public UserDtoRequestProfiler()
        {
            this.CreateMap<UserDtoCreateRequest, User>();
        }
    }
}
