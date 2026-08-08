using ToDoListApp.Models;

namespace ToDoListApp.ViewModels
{
    public class TaskListViewModel
    {
        public IEnumerable<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public string SearchTerm { get; set; } = string.Empty;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;
    }
}