using ToDo.Extensions;
using ToDo.Features.Users.DTO;
using ToDo.Features.Users.Services;

namespace ToDo.Features.Users.Endpoints
{
    public class UserEndpoint : IApiEndpoint
    {
        public void MapEndpoints(IEndpointRouteBuilder routes)
        {
            var userProcess = routes.MapGroup("api/user").WithTags("User");
            userProcess.MapGet("/", async (IUserServices _services) =>
            {
                try
                {
                    var users = await _services.GetAllUsersAsync();
                    return Results.Ok(users);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
                .WithName("GetUserList")
                .WithSummary("Get all users")
                .WithDescription("Get all users.")
                .Produces<List<UserView>>(StatusCodes.Status200OK);
        }
    }
}
