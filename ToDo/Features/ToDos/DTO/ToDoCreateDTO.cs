namespace ToDo.Features.ToDos.DTO
{
    public class ToDoCreateDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public int UserId { get; set; }
    }
}
