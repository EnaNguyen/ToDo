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
        public string Username { get; set; } = string.Empty;
    }
}
