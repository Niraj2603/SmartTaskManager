import { useEffect, useState } from 'react';
import { PieChart, Pie, Cell, Tooltip, Legend, ResponsiveContainer, BarChart, Bar, XAxis, YAxis, CartesianGrid } from 'recharts';
import api from '../api/axios';
import StatCard from '../components/StatCard';

const COLORS = ['#6366f1', '#22c55e', '#f59e0b', '#ef4444', '#06b6d4'];

export default function Dashboard() {
  const [dashboard, setDashboard] = useState(null);
  const [recommendations, setRecommendations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const load = async () => {
      try {
        const [dashRes, recRes] = await Promise.all([
          api.get('/tasks/dashboard'),
          api.get('/recommendations'),
        ]);
        setDashboard(dashRes.data);
        setRecommendations(recRes.data);
      } catch (err) {
        setError('Could not load dashboard data.');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  if (loading) return <div className="page-container">Loading dashboard...</div>;
  if (error) return <div className="page-container error-msg">{error}</div>;
  if (!dashboard) return null;

  const categoryData = Object.entries(dashboard.tasksByCategory || {}).map(([name, value]) => ({ name, value }));
  const priorityData = Object.entries(dashboard.tasksByPriority || {}).map(([name, value]) => ({ name, value }));

  return (
    <div className="page-container">
      <h1>Dashboard</h1>

      <div className="stats-grid">
        <StatCard label="Total Tasks" value={dashboard.totalTasks} />
        <StatCard label="Completed" value={dashboard.completedTasks} accent="accent-green" />
        <StatCard label="Pending" value={dashboard.pendingTasks} accent="accent-orange" />
        <StatCard label="Overdue" value={dashboard.overdueTasks} accent="accent-red" />
        <StatCard label="Productivity" value={`${dashboard.productivityPercentage}%`} accent="accent-purple" />
      </div>

      <div className="charts-grid">
        <div className="chart-card">
          <h3>Tasks by Category</h3>
          <ResponsiveContainer width="100%" height={260}>
            <PieChart>
              <Pie data={categoryData} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={90} label>
                {categoryData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        </div>

        <div className="chart-card">
          <h3>Tasks by Priority</h3>
          <ResponsiveContainer width="100%" height={260}>
            <BarChart data={priorityData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis allowDecimals={false} />
              <Tooltip />
              <Bar dataKey="value" fill="#6366f1" radius={[6, 6, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      <div className="recommendations-card">
        <h3>Recommendations</h3>
        {recommendations.length === 0 ? (
          <p>No recommendations right now.</p>
        ) : (
          <ul>
            {recommendations.map((rec, idx) => (
              <li key={idx}>{rec}</li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
