using AutoMapper;
using Mist.Auth.Application.ViewModels;
using Mist.Auth.Domain.Entities;

namespace Mist.Auth.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<RegisterUserViewModel, User>();
        }
    }
}
