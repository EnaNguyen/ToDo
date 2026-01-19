using ToDo.Data.Entities;
using ToDo.Features.Users.DTO;
using AutoMapper;
namespace ToDo.Features.Users.Services
{
    public class UserServices : IUserServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public UserServices(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<UserView>> GetAllUsersAsync()
        {
            var _users = _context.Users.ToList();
            return _mapper.Map<List<UserView>>(_users);
        }
    }
}
