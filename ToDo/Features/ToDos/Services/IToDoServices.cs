using Microsoft.AspNetCore.Mvc;
using ToDo.Features.ToDos.DTO;
namespace ToDo.Features.ToDos.Services
{
    public interface IToDoServices
    {
        Task<List<ToDoView>> GetAllToDosAsync();
        Task<List<ToDoView>> GetToDoAsync(string username);
        Task<ToDoView> CreateToDoAsync([FromBody] ToDoCreateDTO toDoCreateDTO);
        Task<ToDoView> UpdateToDoAsync([FromBody] ToDoUpdateDTO toDoUpdateDTO);
        Task RemoveToDoAsync(int id);
        Task<ToDoView> FinishToDoAsync(int Id);
        Task<List<ToDoView>> ToDoFilter(ToDoFilter toDoFilter);
    }
}
