import axios from 'axios';

// 1. Config Defaults - כתובת בסיס לשרת ב-Render
axios.defaults.baseURL = "https://todoapi-7m69.onrender.com";

const apiUrl = "/api/todoitems";

// interceptor - שולח token בכל בקשה
axios.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

// interceptor - טיפול בשגיאות
axios.interceptors.response.use(
    response => response,
    error => {
        console.log('Error:', error.response?.status, error.message);
        if (error.response?.status === 401) {
            localStorage.removeItem('token');
            window.location.reload(); 
        }
        return Promise.reject(error);
    }
);

export const authService = {
    register: async (username, password) => {
        await axios.post('/api/auth/register', { username, password });
    },
    login: async (username, password) => {
        const result = await axios.post('/api/auth/login', { username, password });
        localStorage.setItem('token', result.data.token);
    },
    logout: () => localStorage.removeItem('token')
};

const todoService = {
    getTasks: async () => {
        const result = await axios.get(apiUrl);
        return result.data;
    },
    addTask: async (name) => {
        const result = await axios.post(apiUrl, { name, isComplete: false });
        return result.data;
    },
    // עדכון: שולחים את האובייקט המלא כדי לא לדרוס את השם ב-DB
    setCompleted: async (id, name, isComplete) => {
        const result = await axios.put(`${apiUrl}/${id}`, { id, name, isComplete });
        return result.data;
    },
    deleteTask: async (id) => {
        const result = await axios.delete(`${apiUrl}/${id}`);
        return result.data;
    }
};

export default todoService;