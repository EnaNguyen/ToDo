using ToDo.Features.Users.DTO;
namespace ToDo.Features.Users.Services
{
    public interface IUserServices
    {
        Task<List<UserView>> GetAllUsersAsync();
        Task<UserView> GetUserByUsernameAsync(string username);
    }
}
