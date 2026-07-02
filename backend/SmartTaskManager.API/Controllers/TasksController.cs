using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTaskManager.API.Data;
using SmartTaskManager.API.DTOs;
using SmartTaskManager.API.Models;
using SmartTaskManager.API.Services;
using System.Security.Claims;

namespace SmartTaskManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly PriorityService _priorityService;

        public TasksController(AppDbContext db, PriorityService priorityService)
        {
            _db = db;
            _priorityService = priorityService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET /api/tasks?search=&status=&priority=&categoryId=&dueDate=
        [HttpGet]
        public async Task<IActionResult> GetTasks(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] string? priority,
            [FromQuery] int? categoryId,
            [FromQuery] DateTime? dueDate)
        {
            var userId = GetUserId();
            var query = _db.Tasks.Include(t => t.Category).Where(t => t.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t => t.Title.Contains(search) || (t.Category != null && t.Category.CategoryName.Contains(search)));

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Models.TaskStatus>(status, true, out var statusEnum))
                query = query.Where(t => t.Status == statusEnum);

            if (!string.IsNullOrWhiteSpace(priority) && Enum.TryParse<ImportanceLevel>(priority, true, out var importanceEnum))
                query = query.Where(t => t.Importance == importanceEnum);

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId);

            if (dueDate.HasValue)
                query = query.Where(t => t.DueDate.Date == dueDate.Value.Date);

            var tasks = await query.ToListAsync();
            var result = tasks.Select(MapToDto).OrderByDescending(t => t.PriorityScore).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var userId = GetUserId();
            var task = await _db.Tasks.Include(t => t.Category).FirstOrDefaultAsync(t => t.TaskId == id && t.UserId == userId);
            if (task == null) return NotFound();
            return Ok(MapToDto(task));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskCreateDto dto)
        {
            var userId = GetUserId();
            var task = new TaskItem
            {
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description,
                Importance = dto.Importance,
                DueDate = dto.DueDate,
                Status = dto.Status,
                CategoryId = dto.CategoryId,
                CreatedDate = DateTime.UtcNow
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            var saved = await _db.Tasks.Include(t => t.Category).FirstAsync(t => t.TaskId == task.TaskId);
            return CreatedAtAction(nameof(GetTask), new { id = task.TaskId }, MapToDto(saved));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskUpdateDto dto)
        {
            var userId = GetUserId();
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.TaskId == id && t.UserId == userId);
            if (task == null) return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Importance = dto.Importance;
            task.DueDate = dto.DueDate;
            task.Status = dto.Status;
            task.CategoryId = dto.CategoryId;

            await _db.SaveChangesAsync();

            var saved = await _db.Tasks.Include(t => t.Category).FirstAsync(t => t.TaskId == task.TaskId);
            return Ok(MapToDto(saved));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetUserId();
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.TaskId == id && t.UserId == userId);
            if (task == null) return NotFound();

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = GetUserId();
            var tasks = await _db.Tasks.Include(t => t.Category).Where(t => t.UserId == userId).ToListAsync();

            int total = tasks.Count;
            int completed = tasks.Count(t => t.Status == Models.TaskStatus.Completed);
            int pending = total - completed;
            int overdue = tasks.Count(t => _priorityService.IsOverdue(t));
            double productivity = total == 0 ? 0 : Math.Round((double)completed / total * 100, 1);

            var byCategory = tasks
                .GroupBy(t => t.Category?.CategoryName ?? "Uncategorized")
                .ToDictionary(g => g.Key, g => g.Count());

            var byPriority = tasks
                .GroupBy(t => t.Importance.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var dashboard = new DashboardDto
            {
                TotalTasks = total,
                CompletedTasks = completed,
                PendingTasks = pending,
                OverdueTasks = overdue,
                ProductivityPercentage = productivity,
                TasksByCategory = byCategory,
                TasksByPriority = byPriority
            };

            return Ok(dashboard);
        }

        private TaskResponseDto MapToDto(TaskItem task)
        {
            return new TaskResponseDto
            {
                TaskId = task.TaskId,
                Title = task.Title,
                Description = task.Description,
                Importance = task.Importance.ToString(),
                DueDate = task.DueDate,
                Status = task.Status.ToString(),
                CreatedDate = task.CreatedDate,
                CategoryId = task.CategoryId,
                CategoryName = task.Category?.CategoryName,
                PriorityScore = _priorityService.CalculateScore(task),
                IsOverdue = _priorityService.IsOverdue(task),
                PendingDays = _priorityService.GetPendingDays(task)
            };
        }
    }
}
