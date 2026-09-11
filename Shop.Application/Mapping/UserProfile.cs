using AutoMapper;
using Shop.Application.DTOs.UserDTOs;
using Shop.Domain.Models;

namespace Shop.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserCreateDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        CreateMap<User, UserReadDTO>();

        CreateMap<User, UserLoginDTO>();

        CreateMap<User, UserTokenDTO>();

        CreateMap<UserAddress, UserAddressReadDTO>();
    }
}
