using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using ToDo.Features.ToDos.DTO;
using ToDo.Features.ToDos.Services;
using FluentValidation;
using ToDo.Extensions;
using Microsoft.AspNetCore.Authorization;
namespace ToDo.Features.ToDos.Endpoints
{
    public class ToDoEndpoint : IApiEndpoint
    {
        public void MapEndpoints(IEndpointRouteBuilder routes)
        {        
            var ToDoProcess = routes.MapGroup("api/ToDo").WithTags("ToDo");
            ToDoProcess.MapGet("/", async (IToDoServices _services) =>
            {
                try
                {
                    var toDoViews = await _services.GetAllToDosAsync();
                    return Results.Ok(toDoViews);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
                .WithName("GetToDoList")
                .WithSummary("Get all ToDo items")
                .WithDescription("Get all ToDo items.")
                .Produces<List<ToDoView>>(StatusCodes.Status200OK);
            ToDoProcess.MapGet("/{username}", async (IToDoServices _services, string username) =>
            {
                try
                {
                    var toDoViews = await _services.GetToDoAsync(username);
                    return Results.Ok(toDoViews);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
                .WithName("GetToDo")
                .WithSummary("Get ToDo item by username")
                .WithDescription("Get a ToDo item by owner.")
                .RequireAuthorization("SameUsernamePolicy")
                .Produces<ToDoView>(StatusCodes.Status200OK);
            ToDoProcess.MapPost("/", async (IToDoServices _services, [FromBody] ToDoCreateDTO toDoCreateDTO) =>
            {
                try
                {
                    var create = await _services.CreateToDoAsync(toDoCreateDTO);
                    return Results.CreatedAtRoute("GetToDo", new { id = create.Id }, create);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
                .WithName("CreateToDo")
                .WithSummary("Create a new ToDo item")
                .WithDescription("Creates a new ToDo item with the provided details.")
                .Produces<ToDo.Data.Entities.ToDo>(StatusCodes.Status201Created);
            ToDoProcess.MapPut("/{id:int}", async (IToDoServices _services, [FromBody] ToDoUpdateDTO toDoUpdateDTO, int id, IMapper _mapper) =>
            {
                try
                {
                    var update = await _services.UpdateToDoAsync(toDoUpdateDTO, id);
                    var ToDoView = _mapper.Map<ToDoView>(update);
                    return Results.Ok(ToDoView);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }

            })
                .WithName("UpdateToDo")
                .WithSummary("Update ToDo")
                .WithDescription("Update ToDo by Id")
                .Produces<ToDo.Data.Entities.ToDo>(StatusCodes.Status200OK);
            ToDoProcess.MapDelete("/{id:int}", async (IToDoServices _services, int id) =>
            {
                try
                {
                    await _services.RemoveToDoAsync(id);
                    return Results.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return Results.NotFound();
                }
                catch (ValidationException ex1)
                {
                    return Results.BadRequest(ex1.Message);
                }
                catch (Exception ex2)
                {

                    return Results.Problem(
                        detail: ex2.Message,
                        statusCode: 500,
                        title: "Internal Server Error"
                    );
                }
            })
                .WithName("DeleteToDo")
                .WithSummary("Delete a ToDo item by ID")
                .WithDescription("Permanently removes a ToDo item.")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);
        }
    }
}
