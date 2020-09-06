using AutoMapper;
using Mist.Auth.Application.ViewModels;
using Mist.Auth.Domain.Entities;

namespace Auth.Api.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<RegisterUserViewModel, User>();
        }
    }
}
