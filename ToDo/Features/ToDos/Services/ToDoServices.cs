using AutoMapper;
using Azure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.Data.Entities;
using ToDo.Features.ToDos.DTO;

namespace ToDo.Features.ToDos.Services
{
    public class ToDoServices :IToDoServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<ToDoCreateDTO> _validator;
        public ToDoServices(ApplicationDbContext context, IMapper mapper, IValidator<ToDoCreateDTO> validator   )
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }


        public async Task<List<ToDoView>> GetAllToDosAsync()
        {
            var toDoItems = _context.ToDoItems.Include(t => t.User).ToList();
            ToDoView[] toDoViews = _mapper.Map<ToDoView[]>(toDoItems);
            return toDoViews.ToList();
        }

        public async Task<ToDoView> CreateToDoAsync([FromBody] ToDoCreateDTO toDoCreateDTO)
        {
            var validationResult = _validator.ValidateAsync(toDoCreateDTO).GetAwaiter().GetResult();
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            try
            {
                Data.Entities.ToDo toDoEntity = _mapper.Map<Data.Entities.ToDo>(toDoCreateDTO);
                toDoEntity.CreatedAt = DateTime.Now;
                toDoEntity.IsCompleted = false;
                _context.ToDoItems.Add(toDoEntity);
                await _context.SaveChangesAsync();
                return _mapper.Map<ToDoView>(toDoEntity);
            }
            catch (Exception ex)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public async Task<List<ToDoView>> GetToDoAsync(string username)
        {
            var user =  _context.Users.FirstOrDefault(g => g.Username == username);
            var listToDo = _context.ToDoItems.Include(g => g.User).Where(h => h.UserId == user.Id);
            List<ToDoView> toDoView = _mapper.Map<List<ToDoView>>(listToDo);
            return toDoView;
        }
        public async Task<ToDoView> UpdateToDoAsync([FromBody] ToDoUpdateDTO toDoUpdateDTO, int id)
        {
            var toDoEntity =  _context.ToDoItems.FirstOrDefault(g=>g.Id==id);
            if (toDoEntity == null)
            {
                throw new ValidationException("ToDo item not found.");
            }
            try
            {
                _mapper.Map(toDoUpdateDTO, toDoEntity);
                _context.ToDoItems.Update(toDoEntity);
                await _context.SaveChangesAsync();
                return _mapper.Map<ToDoView>(toDoEntity);
            }
            catch(Exception ex)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public async Task RemoveToDoAsync(int id)
        {
            try
            {
                var toDoRemove = await _context.ToDoItems
                    .FirstOrDefaultAsync(g => g.Id == id);
                if (toDoRemove == null)
                {
                    throw new KeyNotFoundException("ToDo item not found.");
                    
                }

                _context.ToDoItems.Remove(toDoRemove);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Can't remove this item");  
            }
        }
    }
}
