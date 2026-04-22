import React, { useState } from 'react';
import { authService } from './service';

function Login({ onLogin, onGoToRegister }) {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleLogin = async () => {
        if (!username || !password) {
            setError('נא למלא את כל השדות');
            return;
        }
        setLoading(true);
        try {
            await authService.login(username, password);
            onLogin();
        } catch {
            setError('שם משתמש או סיסמה שגויים');
        }
        setLoading(false);
    };

    return (
        <div style={styles.page}>
            <div style={styles.card}>
                <h2 style={styles.title}>👋 התחברות</h2>
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
                    onKeyDown={e => e.key === 'Enter' && handleLogin()}
                />
                {error && <p style={styles.error}>{error}</p>}
                <button style={styles.button} onClick={handleLogin} disabled={loading}>
                    {loading ? 'מתחבר...' : 'התחבר'}
                </button>
                 <button
                style={styles.registerLink}
                onClick={onGoToRegister}
            >
                אין לך חשבון? הירשם כאן
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
        backgroundColor: '#4a90d9',
        color: 'white',
        fontSize: '16px',
        cursor: 'pointer',
        fontWeight: 'bold',
    },
    error: {
        color: 'red',
        textAlign: 'center',
        margin: 0,
    },
    registerLink: {
        background: 'none',
        border: 'none',
        color: '#4a90d9',
        cursor: 'pointer',
        textAlign: 'center',
        fontSize: '14px',
    }
};

export default Login;