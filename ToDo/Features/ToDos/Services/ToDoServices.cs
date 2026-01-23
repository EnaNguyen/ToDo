using AutoMapper;
using Azure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
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
                var newToDo = _context.ToDoItems.Include(h => h.User).FirstOrDefault(g => g.Id == toDoEntity.Id);
                return _mapper.Map<ToDoView>(newToDo);
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
        public async Task<ToDoView> UpdateToDoAsync([FromBody] ToDoUpdateDTO toDoUpdateDTO)
        {
            var toDoEntity =  _context.ToDoItems.Include(h=>h.User).FirstOrDefault(g=>g.Id==toDoUpdateDTO.Id);
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

        public async Task<ToDoView> FinishToDoAsync(int Id)
        {
            try
            {
                var toDoFinish = _context.ToDoItems.Include(h => h.User).FirstOrDefault(g => g.Id == Id);
                if (toDoFinish == null)
                {
                    throw new ValidationException("ToDo item not found.");
                }
                toDoFinish.IsCompleted = true;
                _context.ToDoItems.Update(toDoFinish);
                await _context.SaveChangesAsync();
                return _mapper.Map<ToDoView>(toDoFinish);
            }
            catch (Exception ex)
            {
                throw new ValidationException(ex.Message);
            }
        }
        public async Task<List<ToDoView>> ToDoFilter (ToDoFilter toDoFilter)
        {
            try
            {
                var toDoQuery = _context.ToDoItems.Include(g=>g.User); 
                List<ToDoView> toDoListResponse = new List<ToDoView>();
                if(toDoFilter.Comboboxes!=null)
                {
                    foreach(var item in toDoFilter.Comboboxes)
                    {
                        if(!string.IsNullOrEmpty(item.Label)& !string.IsNullOrEmpty(item.Value))
                        {
                            if(item.Value!="true"&& item.Value!="false")
                            {
                                var toDoUnit = toDoQuery.Where(todo => EF.Property<string>(todo, item.Label).ToLower().Trim() == item.Value.ToLower().Trim()).ToList();
                                toDoListResponse.AddRange(_mapper.Map<List<ToDoView>>(toDoUnit));
                            }
                            else
                            {
                                var toDoUnit = toDoQuery.Where(todo => EF.Property<bool>(todo, item.Label) == (item.Value == "true" ? true : false)).ToList();
                                toDoListResponse.AddRange(_mapper.Map<List<ToDoView>>(toDoUnit));
                            } 
                                
                        }    
                    }    
                }
                if(!string.IsNullOrEmpty(toDoFilter.SearchInput))
                {
                    if(toDoListResponse!=null && toDoListResponse.Count()>0)
                    {
                        toDoListResponse = toDoListResponse.Where(g => g.Title.Contains(toDoFilter.SearchInput) || g.Description.Contains(toDoFilter.SearchInput)).ToList();
                    }
                    else
                    {
                        toDoListResponse.AddRange( _mapper.Map<List<ToDoView>> ((toDoQuery.Where(g => g.Title == toDoFilter.SearchInput || g.Description == toDoFilter.SearchInput).ToList())));
                    }    
                }
                if (toDoFilter.SortOptions != null)
                {
                    var Target = new List<ToDoView>();
                    if (toDoListResponse != null && toDoListResponse.Count() > 0)
                    {
                        Target = toDoListResponse;
                    }
                    else
                    {
                        Target = _mapper.Map<List<ToDoView>>(toDoQuery);
                    }
                    if (toDoFilter.SortOptions.type.Trim() == "date")
                    {
                        Target = Target.OrderByDescending(g => EF.Property<DateOnly>(g, toDoFilter.SortOptions.SortByTitle)).ToList();
                    }
                    else if (toDoFilter.SortOptions.type == "String")
                    {
                        Target = Target.OrderByDescending(g => EF.Property<string>(g, toDoFilter.SortOptions.SortByTitle)).ToList();
                    }
                    else
                    {
                        Target = Target.OrderByDescending(g => EF.Property<int>(g, toDoFilter.SortOptions.SortByTitle)).ToList();
                    }
                    if(!toDoFilter.SortOptions.IsDescending)
                    {
                        Target = Target.AsEnumerable().Reverse().ToList();
                    }
                    toDoListResponse = Target;
                }
                if(toDoFilter.RangeFilters!=null)
                {
                    var Target = new List<ToDoView>();
                    if (toDoListResponse != null && toDoListResponse.Count() > 0)
                    {
                        Target = toDoListResponse;
                    }
                    else
                    {
                        Target = _mapper.Map<List<ToDoView>>(toDoQuery);
                    }
                    foreach(var item in toDoFilter.RangeFilters)
                    {
                        var propertyInfo = typeof(ToDoView).GetProperty(item.Target);
                        if(propertyInfo!=null)
                        {
                            if (item.WhichType.ToLower().Trim() == "date")
                            {
                                Target = Target.Where(g => {
                                    var value = propertyInfo.GetValue(g);
                                    if(value is DateTime dateTime)
                                    {
                                        if (dateTime > DateTime.Parse(item.End) && dateTime < DateTime.Parse(item.Start))
                                            return true;
                                        else
                                            return false;
                                    }
                                    return false;
                                }).ToList();
                            }
                            if (item.WhichType.ToLower().Trim() == "number")
                            {
                                Target = Target.Where(g => {
                                    var value = propertyInfo.GetValue(g);
                                    if (value is int numberCompare)
                                    {
                                        if (numberCompare > int.Parse(item.End) && numberCompare < int.Parse(item.Start))
                                            return true;
                                        else
                                            return false;
                                    }
                                    return false;
                                }).ToList();
                            }
                        }    
                    }
                    toDoListResponse = Target;
                }
                return toDoListResponse;
            }
            catch(Exception ex)
            {
                throw new ValidationException(ex.Message);
            }
        }
    }
}
