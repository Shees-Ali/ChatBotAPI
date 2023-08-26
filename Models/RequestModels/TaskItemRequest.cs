using System.ComponentModel.DataAnnotations;

namespace ChatBotAPI.Models.RequestModels
{
    public class TaskItemRequest
    {
        [Required(ErrorMessage = "Task Name is required")]
        public string? TaskName { get; set; }
        [Required(ErrorMessage = "Task Description is required")]
        public string? TaskDescription { get; set; }
        [Required(ErrorMessage = "Task Type is required")]
        public string? TaskType { get; set; }
        [Required(ErrorMessage = "Task Status is required")]
        public string? TaskStatus { get; set; }
        public string TaskTags { get; set; } = string.Empty;
        [Required(ErrorMessage = "Task Date is required")]
        public string? TaskDate { get; set; }
    }
}
