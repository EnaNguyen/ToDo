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
            var users = _context.Users.ToList();
            return _mapper.Map<List<UserView>>(users);
        }

        public async Task<UserView> GetUserByUsernameAsync(string username)
        {
            var user = _context.Users.FirstOrDefault(g => g.Username == username);
            return _mapper.Map<UserView>(user);
        }
    }
}
