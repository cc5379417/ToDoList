import React, { useState } from 'react';
import { authService } from './service';

function Register({ onRegister }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleRegister = async () => {
    if (!username || !password) {
      setError('נא למלא את כל השדות');
      return;
    }
    setLoading(true);
    try {
      await authService.register(username, password);
      onRegister();
    } catch {
      setError('שגיאה בהרשמה, נסה שם משתמש אחר');
    }
    setLoading(false);
  };

  return (
    <div style={styles.page}>
      <div style={styles.card}>
        <h2 style={styles.title}>✨ הרשמה</h2>
        <input
          style={styles.input}
          placeholder="שם משתמש"
          value={username}
          onChange={e => setUsername(e.target.value)}
        />
        <input
          style={styles.input}
          placeholder="סיסמה"
          type="password"
          value={password}
          onChange={e => setPassword(e.target.value)}
          onKeyDown={e => e.key === 'Enter' && handleRegister()}
        />
        {error && <p style={styles.error}>{error}</p>}
        <button style={styles.button} onClick={handleRegister} disabled={loading}>
          {loading ? 'נרשם...' : 'הירשם'}
        </button>
      </div>
    </div>
  );
}

const styles = {
  page: {
    minHeight: '100vh',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#f0f2f5',
    fontFamily: 'Arial, sans-serif',
    direction: 'rtl',
  },
  card: {
    backgroundColor: 'white',
    padding: '40px',
    borderRadius: '12px',
    boxShadow: '0 4px 20px rgba(0,0,0,0.1)',
    display: 'flex',
    flexDirection: 'column',
    gap: '16px',
    width: '320px',
  },
  title: {
    textAlign: 'center',
    color: '#333',
    marginBottom: '8px',
  },
  input: {
    padding: '12px',
    borderRadius: '8px',
    border: '1px solid #ddd',
    fontSize: '16px',
    outline: 'none',
    textAlign: 'right',
  },
  button: {
    padding: '12px',
    borderRadius: '8px',
    border: 'none',
    backgroundColor: '#5cb85c',
    color: 'white',
    fontSize: '16px',
    cursor: 'pointer',
    fontWeight: 'bold',
  },
  error: {
    color: 'red',
    textAlign: 'center',
    margin: 0,
  }
};

export default Register;