using SmartTaskManager.API.Models;

namespace SmartTaskManager.API.Services
{
    public class RecommendationService
    {
        private readonly PriorityService _priorityService;

        public RecommendationService(PriorityService priorityService)
        {
            _priorityService = priorityService;
        }

        public List<string> GenerateRecommendations(List<TaskItem> tasks)
        {
            var recommendations = new List<string>();
            var activeTasks = tasks.Where(t => t.Status != Models.TaskStatus.Completed).ToList();

            if (!activeTasks.Any())
            {
                recommendations.Add("You're all caught up! No pending tasks right now.");
                return recommendations;
            }

            // Rule 1: Overdue tasks
            var overdueTasks = activeTasks.Where(t => _priorityService.IsOverdue(t)).ToList();
            if (overdueTasks.Any())
            {
                recommendations.Add($"Complete {overdueTasks.Count} overdue task(s) immediately, especially \"{overdueTasks.OrderBy(t => t.DueDate).First().Title}\".");
            }

            // Rule 2: Top priority task (due soon / near deadline)
            var topTask = activeTasks.OrderByDescending(t => _priorityService.CalculateScore(t)).First();
            if (!overdueTasks.Contains(topTask))
            {
                recommendations.Add($"Complete \"{topTask.Title}\" first because its deadline is near and importance is high.");
            }

            // Rule 3: High priority task count
            var highPriorityCount = activeTasks.Count(t => t.Importance == ImportanceLevel.High);
            if (highPriorityCount >= 3)
            {
                recommendations.Add($"You have {highPriorityCount} pending high-priority tasks. Consider tackling these before anything else.");
            }

            // Rule 4: Many small/low-importance tasks -> quick wins
            var lowImportanceCount = activeTasks.Count(t => t.Importance == ImportanceLevel.Low);
            if (lowImportanceCount >= 2)
            {
                recommendations.Add("Try finishing small, low-importance tasks first to build momentum and increase productivity.");
            }

            // Rule 5: Due today
            var dueTodayTasks = activeTasks.Where(t => t.DueDate.Date == DateTime.UtcNow.Date).ToList();
            if (dueTodayTasks.Any())
            {
                recommendations.Add($"You have {dueTodayTasks.Count} task(s) due today, don't let them slip.");
            }

            // Rule 6: Long pending tasks
            var stalledTasks = activeTasks.Where(t => _priorityService.GetPendingDays(t) >= 7 && t.Status == Models.TaskStatus.ToDo).ToList();
            if (stalledTasks.Any())
            {
                recommendations.Add($"\"{stalledTasks.First().Title}\" has been pending for over a week. Consider starting it today.");
            }

            return recommendations;
        }
    }
}
