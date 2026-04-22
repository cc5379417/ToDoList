import axios from 'axios';

// 1. Config Defaults - כתובת בסיס
axios.defaults.baseURL = "https://todoapi-7m69.onrender.com"

const apiUrl = "/api/todoitems";

// interceptor - שולח token בכל בקשה
axios.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

// interceptor - תופס שגיאות ורושם ללוג + מעביר 401 ללוגין
axios.interceptors.response.use(
    response => response,
    error => {
        console.log('Error:', error.response?.status, error.message);
        if (error.response?.status === 401) {
            localStorage.removeItem('token'); // מחק token ישן
            window.location.reload(); // טען מחדש - יציג לוגין
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

export default {
    getTasks: async () => (await axios.get(apiUrl)).data,
    addTask: async (name) => (await axios.post(apiUrl, { name, isComplete: false })).data,
    setCompleted: async (id, isComplete) => (await axios.put(`${apiUrl}/${id}`, { id, name: "", isComplete })).data,
    deleteTask: async (id) => (await axios.delete(`${apiUrl}/${id}`)).data
};