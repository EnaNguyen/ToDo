namespace ToDo.Features.ToDos.DTO
{
    public class ToDoUpdateDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public int Id { get; set; }
    }
}
