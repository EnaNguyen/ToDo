using ToDo.Features.Users.DTO;
namespace ToDo.Features.Users.Services
{
    public interface IUserServices
    {
        Task<List<UserView>> GetAllUsersAsync();
        Task<UserView> GetUserAsync(int id);
        Task<UserView> CreateUserAsync(UserCreateDTO userCreateDTO);
        Task<UserView> UpdateUserAsync(UserCreateDTO userUpdateDTO, int id);
        Task RemoveUserAsync(int id);
        Task<UserView> ChangePasswordAsync(UserChangePasswordDTO userChangePasswordDTO, int id);
    }
}
