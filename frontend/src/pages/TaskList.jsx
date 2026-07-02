import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/axios';
import TaskCard from '../components/TaskCard';

export default function TaskList() {
  const [tasks, setTasks] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [search, setSearch] = useState('');
  const [status, setStatus] = useState('');
  const [priority, setPriority] = useState('');
  const [categoryId, setCategoryId] = useState('');

  const navigate = useNavigate();

  const loadCategories = useCallback(async () => {
    const res = await api.get('/categories');
    setCategories(res.data);
  }, []);

  const loadTasks = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const params = {};
      if (search) params.search = search;
      if (status) params.status = status;
      if (priority) params.priority = priority;
      if (categoryId) params.categoryId = categoryId;
      const res = await api.get('/tasks', { params });
      setTasks(res.data);
    } catch (err) {
      setError('Could not load tasks.');
    } finally {
      setLoading(false);
    }
  }, [search, status, priority, categoryId]);

  useEffect(() => {
    loadCategories();
  }, [loadCategories]);

  useEffect(() => {
    loadTasks();
  }, [loadTasks]);

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this task?')) return;
    await api.delete(`/tasks/${id}`);
    loadTasks();
  };

  const handleStatusChange = async (id, newStatus) => {
    const task = tasks.find((t) => t.taskId === id);
    if (!task) return;
    await api.put(`/tasks/${id}`, {
      title: task.title,
      description: task.description,
      importance: task.importance,
      dueDate: task.dueDate,
      status: newStatus,
      categoryId: task.categoryId,
    });
    loadTasks();
  };

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>Tasks</h1>
        <button onClick={() => navigate('/tasks/new')}>+ Add Task</button>
      </div>

      <div className="filters-bar">
        <input
          type="text"
          placeholder="Search by title or category..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <select value={status} onChange={(e) => setStatus(e.target.value)}>
          <option value="">All Statuses</option>
          <option value="ToDo">To Do</option>
          <option value="InProgress">In Progress</option>
          <option value="Completed">Completed</option>
        </select>
        <select value={priority} onChange={(e) => setPriority(e.target.value)}>
          <option value="">All Priorities</option>
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
        </select>
        <select value={categoryId} onChange={(e) => setCategoryId(e.target.value)}>
          <option value="">All Categories</option>
          {categories.map((c) => (
            <option key={c.categoryId} value={c.categoryId}>{c.categoryName}</option>
          ))}
        </select>
      </div>

      {loading && <p>Loading tasks...</p>}
      {error && <p className="error-msg">{error}</p>}
      {!loading && tasks.length === 0 && <p>No tasks found. Try adjusting filters or add a new task.</p>}

      <div className="task-list">
        {tasks.map((task) => (
          <TaskCard
            key={task.taskId}
            task={task}
            onDelete={handleDelete}
            onStatusChange={handleStatusChange}
          />
        ))}
      </div>
    </div>
  );
}
