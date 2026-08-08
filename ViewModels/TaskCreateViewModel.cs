using System.ComponentModel.DataAnnotations;

namespace ToDoListApp.ViewModels
{
    public class TaskCreateViewModel
    {
        [Required(ErrorMessage = "يرجى إدخال عنوان المهمة")]
        [Display(Name = "العنوان")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "الموعد النهائي")]
        [DataType(DataType.DateTime)]
        public DateTime? Deadline { get; set; }

        [Display(Name = "الملف المرفق")]
        public IFormFile? FormFile { get; set; }
    }
}