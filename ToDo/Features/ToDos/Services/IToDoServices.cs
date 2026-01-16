using Microsoft.AspNetCore.Mvc;
using ToDo.Features.ToDos.DTO;
namespace ToDo.Features.ToDos.Services
{
    public interface IToDoServices
    {
        Task<List<ToDoView>> GetAllToDosAsync();
        Task<ToDoView> GetToDoAsync(int id);
        Task<ToDoView> CreateToDoAsync([FromBody] ToDoCreateDTO toDoCreateDTO);
        Task<ToDoView> UpdateToDoAsync([FromBody] ToDoUpdateDTO toDoUpdateDTO, int id);
        Task RemoveToDoAsync(int id);
    }
}
