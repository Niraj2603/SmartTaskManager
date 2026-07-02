using System.ComponentModel.DataAnnotations;
using SmartTaskManager.API.Models;

namespace SmartTaskManager.API.DTOs
{
    public class RegisterDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateProfileDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? NewPassword { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class TaskCreateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public ImportanceLevel Importance { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public Models.TaskStatus Status { get; set; } = Models.TaskStatus.ToDo;
        public int? CategoryId { get; set; }
    }

    public class TaskUpdateDto : TaskCreateDto
    {
    }

    public class TaskResponseDto
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Importance { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public double PriorityScore { get; set; }
        public bool IsOverdue { get; set; }
        public int PendingDays { get; set; }
    }

    public class CategoryDto
    {
        [Required, MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;
    }

    public class DashboardDto
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public double ProductivityPercentage { get; set; }
        public Dictionary<string, int> TasksByCategory { get; set; } = new();
        public Dictionary<string, int> TasksByPriority { get; set; } = new();
    }
}
