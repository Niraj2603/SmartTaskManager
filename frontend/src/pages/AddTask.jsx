import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import api from '../api/axios';

const emptyForm = {
  title: '',
  description: '',
  importance: 'Medium',
  dueDate: '',
  status: 'ToDo',
  categoryId: '',
};

export default function AddTask() {
  const [form, setForm] = useState(emptyForm);
  const [categories, setCategories] = useState([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = Boolean(id);

  useEffect(() => {
    api.get('/categories').then((res) => setCategories(res.data));
  }, []);

  useEffect(() => {
    if (isEdit) {
      api.get(`/tasks/${id}`).then((res) => {
        const t = res.data;
        setForm({
          title: t.title,
          description: t.description || '',
          importance: t.importance,
          dueDate: t.dueDate ? t.dueDate.substring(0, 10) : '',
          status: t.status,
          categoryId: t.categoryId || '',
        });
      });
    }
  }, [id, isEdit]);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const payload = {
        ...form,
        categoryId: form.categoryId ? Number(form.categoryId) : null,
      };
      if (isEdit) {
        await api.put(`/tasks/${id}`, payload);
      } else {
        await api.post('/tasks', payload);
      }
      navigate('/tasks');
    } catch (err) {
      setError(err.response?.data?.message || 'Could not save task.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page-container">
      <h1>{isEdit ? 'Edit Task' : 'Add Task'}</h1>
      <form className="task-form" onSubmit={handleSubmit}>
        {error && <div className="error-msg">{error}</div>}

        <label>Title</label>
        <input name="title" value={form.title} onChange={handleChange} required />

        <label>Description</label>
        <textarea name="description" value={form.description} onChange={handleChange} rows={4} />

        <div className="form-row">
          <div>
            <label>Importance</label>
            <select name="importance" value={form.importance} onChange={handleChange}>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
            </select>
          </div>
          <div>
            <label>Status</label>
            <select name="status" value={form.status} onChange={handleChange}>
              <option value="ToDo">To Do</option>
              <option value="InProgress">In Progress</option>
              <option value="Completed">Completed</option>
            </select>
          </div>
        </div>

        <div className="form-row">
          <div>
            <label>Due Date</label>
            <input type="date" name="dueDate" value={form.dueDate} onChange={handleChange} required />
          </div>
          <div>
            <label>Category</label>
            <select name="categoryId" value={form.categoryId} onChange={handleChange}>
              <option value="">Uncategorized</option>
              {categories.map((c) => (
                <option key={c.categoryId} value={c.categoryId}>{c.categoryName}</option>
              ))}
            </select>
          </div>
        </div>

        <button type="submit" disabled={loading}>
          {loading ? 'Saving...' : isEdit ? 'Update Task' : 'Create Task'}
        </button>
      </form>
    </div>
  );
}
