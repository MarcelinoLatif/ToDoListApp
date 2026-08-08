using System.ComponentModel.DataAnnotations;

namespace ToDoListApp.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? Deadline { get; set; }

        public string? FilePath { get; set; }

        public string? OriginalFileName { get; set; }
    }
}