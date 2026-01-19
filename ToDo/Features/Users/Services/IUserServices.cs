using ToDo.Features.Users.DTO;
namespace ToDo.Features.Users.Services
{
    public interface IUserServices
    {
        Task<List<UserView>> GetAllUsersAsync();
    }
}
