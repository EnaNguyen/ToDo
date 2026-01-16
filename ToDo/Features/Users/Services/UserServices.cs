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
        public Task<UserView> ChangePasswordAsync(UserChangePasswordDTO userChangePasswordDTO, int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserView> CreateUserAsync(UserCreateDTO userCreateDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserView>> GetAllUsersAsync()
        {
            var _users = _context.Users.ToList();
            return _mapper.Map<List<UserView>>(_users);
        }

        public Task<UserView> GetUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserView> UpdateUserAsync(UserCreateDTO userUpdateDTO, int id)
        {
            throw new NotImplementedException();
        }
    }
}
