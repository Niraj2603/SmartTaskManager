import { useNavigate } from 'react-router-dom';

const statusColors = {
  ToDo: '#94a3b8',
  InProgress: '#f59e0b',
  Completed: '#22c55e',
};

const importanceColors = {
  Low: '#60a5fa',
  Medium: '#f59e0b',
  High: '#ef4444',
};

export default function TaskCard({ task, onDelete, onStatusChange }) {
  const navigate = useNavigate();

  return (
    <div className={`task-card ${task.isOverdue ? 'overdue' : ''}`}>
      <div className="task-card-header">
        <h3>{task.title}</h3>
        <span className="priority-score">Score: {task.priorityScore}</span>
      </div>
      {task.description && <p className="task-desc">{task.description}</p>}
      <div className="task-meta">
        <span className="badge" style={{ background: importanceColors[task.importance] }}>
          {task.importance}
        </span>
        <span className="badge" style={{ background: statusColors[task.status] }}>
          {task.status}
        </span>
        {task.categoryName && <span className="badge category-badge">{task.categoryName}</span>}
        <span className="due-date">
          Due: {new Date(task.dueDate).toLocaleDateString()}
          {task.isOverdue && <strong className="overdue-tag"> (Overdue)</strong>}
        </span>
      </div>
      <div className="task-actions">
        <select
          value={task.status}
          onChange={(e) => onStatusChange(task.taskId, e.target.value)}
        >
          <option value="ToDo">To Do</option>
          <option value="InProgress">In Progress</option>
          <option value="Completed">Completed</option>
        </select>
        <button onClick={() => navigate(`/tasks/${task.taskId}/edit`)}>Edit</button>
        <button className="btn-danger" onClick={() => onDelete(task.taskId)}>Delete</button>
      </div>
    </div>
  );
}
