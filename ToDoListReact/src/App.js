import React, { useEffect, useState } from 'react';
import { todoService as service } from './service';
import Login from './Login';
import Register from './Register';

function App() {
  const [newTodo, setNewTodo] = useState("");
  const [todos, setTodos] = useState([]);
  const [page, setPage] = useState('login'); // 'login' | 'register' | 'app'

  async function getTodos() {
    const todos = await service.getTasks();
    setTodos(todos);
  }

  async function createTodo(e) {
    e.preventDefault();
    await service.addTask(newTodo);
    setNewTodo("");
    await getTodos();
  }

 async function updateCompleted(todo, isComplete) {
    try {
      console.log("Updating todo:", todo.id, todo.name, isComplete);
      
      // כאן הייתה השגיאה: השתמשת ב-todoService במקום ב-service
      await service.setCompleted(todo.id, todo.name, isComplete); 
      
      await getTodos(); 
    } catch (error) {
      console.error("Update failed:", error);
    }
  }

  async function deleteTodo(id) {
    await service.deleteTask(id);
    await getTodos();
  }

  useEffect(() => {
    if (page === 'app') getTodos();
  }, [page]);

  // בדיקה אם כבר מחובר
  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) setPage('app');
  }, []);

  if (page === 'login') {
    return (
      <div>
        <Login onLogin={() => setPage('app')} onGoToRegister={() => setPage('register')} />
        <p style={{ textAlign: 'center' }}>
          אין לך חשבון?{' '}
          <button onClick={() => setPage('register')}>הירשם</button>
        </p>
      </div>
    );
  }

  if (page === 'register') {
    return (
      <div>
        <Register onRegister={() => setPage('login')} />
        <p style={{ textAlign: 'center' }}>
          יש לך חשבון?{' '}
          <button onClick={() => setPage('login')}>התחבר</button>
        </p>
      </div>
    );
  }

  return (
    <section className="todoapp">
      <header className="header">
        <h1>todos</h1>
        <button onClick={() => { localStorage.removeItem('token'); setPage('login'); }}
          style={{ float: 'left', margin: '10px' }}>
          התנתק
        </button>
        <form onSubmit={createTodo}>
          <input className="new-todo" placeholder="Well, let's take on the day"
            value={newTodo} onChange={(e) => setNewTodo(e.target.value)} />
        </form>
      </header>
      <section className="main" style={{ display: "block" }}>
        <ul className="todo-list">
          {todos.map(todo => (
            <li className={todo.isComplete ? "completed" : ""} key={todo.id}>
              <div className="view">
                <input className="toggle" type="checkbox"
                  defaultChecked={todo.isComplete}
                  onChange={(e) => updateCompleted(todo, e.target.checked)} />
                <label>{todo.name}</label>
                <button className="destroy" onClick={() => deleteTodo(todo.id)}></button>
              </div>
            </li>
          ))}
        </ul>
      </section>
    </section>
  );
}

export default App;