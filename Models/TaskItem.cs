using System.ComponentModel.DataAnnotations;

namespace ChatBotAPI.Models
{
    public class TaskItem
    {
        [Key]
        public int id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string TaskType { get; set; } = string.Empty;
        public string TaskStatus { get; set; } = string.Empty;
        public string TaskTags { get; set;} = string.Empty;
        public string TaskDate { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

    }
}
