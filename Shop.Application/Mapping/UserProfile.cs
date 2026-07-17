using AutoMapper;
using Shop.Application.DTOs.UserDTOs;

namespace Shop.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserCreateDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        CreateMap<User, UserReadDTO>();
        CreateMap<UserUpdateDTO, User>(); //Потім переробити схоже як в CategoryProfile (якщо треба буде)

        CreateMap<User, UserLoginDTO>();
        CreateMap<User, UserTokenDTO>();
    }
}
