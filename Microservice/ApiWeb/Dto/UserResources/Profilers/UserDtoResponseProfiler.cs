using AutoMapper;
using Microservice.ApiWeb.Dto.UserResources.Response;
using Microservice.Domain.Models;

namespace Microservice.ApiWeb.Dto.UserResources.Profilers
{
    public class UserDtoResponseProfiler : Profile
    {
        public UserDtoResponseProfiler()
        {
            this.CreateMap<User, UserDtoGetResponse>()
                .ForMember(dto => dto.UserName, opt => opt.MapFrom(obj => obj.UserName == null ? null : obj.UserName.TrimEnd().TrimStart()))
                .ForMember(dto => dto.Name, opt => opt.MapFrom(obj => obj.Name == null ? null : obj.Name.TrimEnd().TrimStart()))
                .ForMember(dto => dto.Surname, opt => opt.MapFrom(obj => obj.Surname == null ? null : obj.Surname.TrimEnd().TrimStart()));
        }
    }
}
