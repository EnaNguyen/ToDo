namespace ToDo.Features.ToDos.DTO
{
    public class ToDoFilter
    {
        public string? SearchInput { get; set; }
        public SortOptions? SortOptions { get; set; }
        public List<RangeFilter>? RangeFilters { get; set; }
        public List<Combobox>? Comboboxes { get; set; }
        public List<Selections>? Selections { get; set; }
        public Pagination? Pagination { get; set; }
    }
    public class SortOptions
    {
        public string SortByTitle { get; set; }
        public bool IsDescending { get; set; }
        public string WhichType { get; set; }
    }
    public class RangeFilter
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string Target { get; set; }
        public string WhichType { get; set; }
    }
    public class Combobox 
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
    public class Pagination
    {
        public int IndexPage { get; set; }
        public int ItemsPerPage { get; set; }
    }
    public class Selections
    {
        public string Value { get; set; }
        public string Target { get; set; }
    }
}
