using AutoMapper;

namespace ToDo.Features.Users.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<Data.Entities.Users, DTO.UserView>().ReverseMap();
            CreateMap<Data.Entities.Users, DTO.UserCreateDTO>().ReverseMap();
            CreateMap<Data.Entities.Users, DTO.UserUpdateDTO>().ReverseMap();
        }
    }
}
