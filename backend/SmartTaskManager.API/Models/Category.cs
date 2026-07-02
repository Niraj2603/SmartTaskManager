using System.ComponentModel.DataAnnotations;

namespace SmartTaskManager.API.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public List<TaskItem> Tasks { get; set; } = new();
    }
}
