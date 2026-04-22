import axios from 'axios';

// כתובת בסיס דינמית
axios.defaults.baseURL = window.location.hostname === "localhost" 
    ? "http://localhost:5001" 
    : "https://todoapi-7m69.onrender.com";

const apiUrl = "/api/todoitems";

// Interceptors (נשארים אותו דבר)
axios.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

axios.interceptors.response.use(
    response => response,
    error => {
        if (error.response?.status === 401) {
            localStorage.removeItem('token');
            window.location.reload(); 
        }
        return Promise.reject(error);
    }
);

// הגדרת authService (בלי המילה export בהתחלה!)
const authService = {
    register: async (username, password) => {
        await axios.post('/api/auth/register', { username, password });
    },
    login: async (username, password) => {
        const result = await axios.post('/api/auth/login', { username, password });
        localStorage.setItem('token', result.data.token);
    },
    logout: () => localStorage.removeItem('token')
};

// הגדרת todoService
const todoService = {
    getTasks: async () => {
        const result = await axios.get(apiUrl);
        return result.data;
    },
    addTask: async (name) => {
        const result = await axios.post(apiUrl, { name, isComplete: false });
        return result.data;
    },
   setCompleted: async (id, name, isComplete) => {
    // שלחי את האובייקט עם אותיות גדולות בדיוק כמו ב-C#
    const result = await axios.put(`${apiUrl}/${id}`, { 
        Id: id, 
        Name: name, 
        IsComplete: isComplete 
    });
    return result.data;
},
    deleteTask: async (id) => {
        const result = await axios.delete(`${apiUrl}/${id}`);
        return result.data;
    }
};

// ייצוא יחיד ומסודר של שניהם
export { todoService, authService };