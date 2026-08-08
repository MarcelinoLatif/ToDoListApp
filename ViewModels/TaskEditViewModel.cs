using System.ComponentModel.DataAnnotations;

namespace ToDoListApp.ViewModels
{
    public class TaskEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يرجى إدخال عنوان المهمة")]
        [Display(Name = "العنوان")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "الموعد النهائي")]
        [DataType(DataType.DateTime)]
        public DateTime? Deadline { get; set; }

        public string? ExistingFilePath { get; set; }
        public string? ExistingFileName { get; set; }

        [Display(Name = "رفع ملف جديد")]
        public IFormFile? NewFile { get; set; }
    }
}