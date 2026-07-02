using SmartTaskManager.API.Models;

namespace SmartTaskManager.API.Services
{
    public class PriorityService
    {
        // Priority Score = (Importance x 50) + (Urgency x 30) + (PendingDays x 20)
        // Each factor is normalized to a 0..1 range before weighting, so the
        // final score lands between 0 and 100.
        public double CalculateScore(TaskItem task)
        {
            double importanceNormalized = task.Importance switch
            {
                ImportanceLevel.High => 1.0,
                ImportanceLevel.Medium => 0.66,
                ImportanceLevel.Low => 0.33,
                _ => 0.5
            };

            double daysUntilDue = (task.DueDate.Date - DateTime.UtcNow.Date).TotalDays;
            double urgencyNormalized;
            if (daysUntilDue <= 0)
            {
                urgencyNormalized = 1.0; // overdue or due today = max urgency
            }
            else
            {
                // Tasks due within 30 days scale linearly; beyond that, urgency floors out near 0
                urgencyNormalized = Math.Max(0, 1 - (daysUntilDue / 30.0));
            }

            double pendingDays = (DateTime.UtcNow.Date - task.CreatedDate.Date).TotalDays;
            double pendingDaysNormalized = Math.Min(1.0, Math.Max(0, pendingDays) / 30.0);

            double score = (importanceNormalized * 50) + (urgencyNormalized * 30) + (pendingDaysNormalized * 20);
            return Math.Round(Math.Min(100, score), 1);
        }

        public int GetPendingDays(TaskItem task)
        {
            return Math.Max(0, (int)(DateTime.UtcNow.Date - task.CreatedDate.Date).TotalDays);
        }

        public bool IsOverdue(TaskItem task)
        {
            return task.Status != Models.TaskStatus.Completed && task.DueDate.Date < DateTime.UtcNow.Date;
        }
    }
}
