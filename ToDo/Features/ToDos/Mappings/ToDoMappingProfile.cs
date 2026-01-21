using AutoMapper;
using ToDo.Features.ToDos.DTO;
namespace ToDo.Features.ToDos.Mappings
{
    public class ToDoMappingProfile : Profile
    {
        public ToDoMappingProfile()
        {
            CreateMap<Data.Entities.ToDo, ToDoView>().ForMember(u => u.FullName, n => n.MapFrom( a => a.User.FullName)).ReverseMap();
            CreateMap<Data.Entities.ToDo, ToDoCreateDTO>().ReverseMap();
            CreateMap<Data.Entities.ToDo, ToDoUpdateDTO>().ReverseMap();
        }
    }
}
