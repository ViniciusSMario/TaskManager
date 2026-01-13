import './style.css';
import { getAllTasks, createTask, updateTask, deleteTask } from './services/taskService.js';
import { createTaskItem } from './components/TaskItem.js';
import { createTaskForm } from './components/TaskForm.js';
import { createTaskFilter } from './components/TaskFilter.js';

let allTasks = [];
let currentFilter = null;

// Elementos do DOM
const app = document.querySelector('#app');

// Criar estrutura da aplicação
function initApp() {
  app.innerHTML = `
    <div class="container">
      <header class="app-header">
        <h1>📋 Task Manager</h1>
        <p>Gerenciador de Tarefas</p>
      </header>

      <div class="app-toolbar">
        <button id="btn-new-task" class="btn btn-primary">➕ Nova Tarefa</button>
        <div id="filter-container"></div>
      </div>

      <div id="loading" class="loading" style="display: none;">
        <div class="spinner"></div>
        <p>Carregando...</p>
      </div>

      <div id="error-message" class="error-message" style="display: none;"></div>

      <div id="tasks-container" class="tasks-grid"></div>

      <div id="empty-state" class="empty-state" style="display: none;">
        <h2>Nenhuma tarefa encontrada</h2>
        <p>Crie sua primeira tarefa clicando no botão acima</p>
      </div>
    </div>
  `;

  // Adicionar filtro
  const filterContainer = document.querySelector('#filter-container');
  filterContainer.appendChild(createTaskFilter(handleFilter));

  // Event listeners
  document.querySelector('#btn-new-task').addEventListener('click', handleNewTask);

  // Carregar tarefas
  loadTasks();
}

// Mostrar loading
function showLoading(show = true) {
  const loading = document.querySelector('#loading');
  loading.style.display = show ? 'flex' : 'none';
}

// Mostrar erro
function showError(message) {
  const errorDiv = document.querySelector('#error-message');
  errorDiv.textContent = message;
  errorDiv.style.display = 'block';
  setTimeout(() => {
    errorDiv.style.display = 'none';
  }, 5000);
}

// Carregar tarefas
async function loadTasks() {
  try {
    showLoading(true);
    allTasks = await getAllTasks();
    renderTasks();
  } catch (error) {
    showError('Erro ao carregar tarefas. Verifique se a API está rodando.');
  } finally {
    showLoading(false);
  }
}

// Renderizar tarefas
function renderTasks() {
  const container = document.querySelector('#tasks-container');
  const emptyState = document.querySelector('#empty-state');
  
  container.innerHTML = '';

  // Filtrar tarefas
  const filteredTasks = currentFilter !== null 
    ? allTasks.filter(task => task.status === currentFilter)
    : allTasks;

  if (filteredTasks.length === 0) {
    emptyState.style.display = 'block';
    container.style.display = 'none';
  } else {
    emptyState.style.display = 'none';
    container.style.display = 'grid';
    
    filteredTasks.forEach(task => {
      const taskItem = createTaskItem(task, handleEditTask, handleDeleteTask);
      container.appendChild(taskItem);
    });
  }
}

// Filtrar tarefas
function handleFilter(status) {
  currentFilter = status;
  renderTasks();
}

// Nova tarefa
function handleNewTask() {
  const form = createTaskForm(null, handleCreateTask);
  document.body.appendChild(form);
}

// Criar tarefa
async function handleCreateTask(taskData) {
  try {
    showLoading(true);
    await createTask(taskData);
    await loadTasks();
  } catch (error) {
    showError(error.message);
  } finally {
    showLoading(false);
  }
}

// Editar tarefa
function handleEditTask(task) {
  const form = createTaskForm(task, (taskData) => handleUpdateTask(task.id, taskData));
  document.body.appendChild(form);
}

// Atualizar tarefa
async function handleUpdateTask(id, taskData) {
  try {
    showLoading(true);
    await updateTask(id, taskData);
    await loadTasks();
  } catch (error) {
    showError(error.message);
  } finally {
    showLoading(false);
  }
}

// Deletar tarefa
async function handleDeleteTask(id) {
  if (!confirm('Tem certeza que deseja deletar esta tarefa?')) {
    return;
  }

  try {
    showLoading(true);
    await deleteTask(id);
    await loadTasks();
  } catch (error) {
    showError(error.message);
  } finally {
    showLoading(false);
  }
}

// Inicializar aplicação
initApp();
