using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTaskManager.API.Models
{
    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Completed
    }

    public enum ImportanceLevel
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public ImportanceLevel Importance { get; set; } = ImportanceLevel.Medium;

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Calculated, not stored - exposed via DTO instead, but kept here for convenience
        [NotMapped]
        public double PriorityScore { get; set; }
    }
}
