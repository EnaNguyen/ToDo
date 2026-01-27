using ToDo.Data.Entities;

namespace ToDo.Features.ToDos.DTO
{
    public class ToDoView
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
    public class ToDoResponse
    {
        public List<ToDoView> ToDoItems { get; set; }
        public List<ToDoView> ItemInPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int PageIndex { get; set; }
    }
}
