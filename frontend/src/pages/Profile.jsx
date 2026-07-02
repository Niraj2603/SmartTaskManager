import { useEffect, useState } from 'react';
import api from '../api/axios';
import { useAuth } from '../context/AuthContext';

export default function Profile() {
  const { user, setUser } = useAuth();
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    api.get('/auth/profile').then((res) => {
      setName(res.data.name);
      setEmail(res.data.email);
    });
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setMessage('');
    setError('');
    try {
      const res = await api.put('/auth/profile', { name, newPassword: newPassword || null });
      const updatedUser = { ...user, name: res.data.name };
      localStorage.setItem('user', JSON.stringify(updatedUser));
      setUser(updatedUser);
      setMessage('Profile updated successfully.');
      setNewPassword('');
    } catch (err) {
      setError(err.response?.data?.message || 'Could not update profile.');
    }
  };

  return (
    <div className="page-container">
      <h1>Profile</h1>
      <form className="task-form" onSubmit={handleSubmit} style={{ maxWidth: 480 }}>
        {message && <div className="success-msg">{message}</div>}
        {error && <div className="error-msg">{error}</div>}

        <label>Name</label>
        <input value={name} onChange={(e) => setName(e.target.value)} required />

        <label>Email</label>
        <input value={email} disabled />

        <label>New Password (optional)</label>
        <input type="password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} placeholder="Leave blank to keep current password" />

        <button type="submit">Save Changes</button>
      </form>
    </div>
  );
}
