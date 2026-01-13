import { getAllTasks, createTask, updateTask, deleteTask } from './services/taskService.js';
import { createTaskItem } from './components/TaskItem.js';
import { createTaskForm } from './components/TaskForm.js';
import { createTaskFilter } from './components/TaskFilter.js';
import './style.css';

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

  // Kanban: colunas por status
  const columns = [
    { status: 0, title: 'Pendente' },
    { status: 1, title: 'Em Progresso' },
    { status: 2, title: 'Concluída' }
  ];

  // Filtrar tarefas
  const filteredTasks = currentFilter !== null
    ? allTasks.filter(task => task.status === currentFilter)
    : allTasks;

  let hasTasks = false;
  const board = document.createElement('div');
  board.className = 'kanban-board-content';

  columns.forEach(col => {
    // Se filtro está ativo, só mostra a coluna do filtro
    if (currentFilter !== null && col.status !== currentFilter) return;
    const colDiv = document.createElement('div');
    colDiv.className = 'kanban-column';
    colDiv.innerHTML = `<h3>${col.title}</h3>`;
    const colTasks = filteredTasks.filter(task => task.status === col.status);
    if (colTasks.length > 0) hasTasks = true;
    colTasks.forEach(task => {
      const taskItem = createTaskItem(task, handleEditTask, handleDeleteTask);
      colDiv.appendChild(taskItem);
    });
    board.appendChild(colDiv);
  });

  if (!hasTasks) {
    emptyState.style.display = 'block';
    container.style.display = 'none';
  } else {
    emptyState.style.display = 'none';
    container.style.display = 'block';
    container.appendChild(board);
  }
}

// Kanban CSS básico
const style = document.createElement('style');
style.innerHTML = `
.kanban-board-content {
  display: flex;
  gap: 1.5rem;
  width: 100%;
}
.kanban-column {
  background: #f7f7fa;
  border-radius: 8px;
  padding: 1rem;
  flex: 1 1 0;
  min-width: 250px;
  box-shadow: 0 2px 8px #0001;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
.kanban-column h3 {
  text-align: center;
  margin-bottom: 1rem;
  color: #555;
}
@media (max-width: 900px) {
  .kanban-board-content {
    flex-direction: column;
  }
  .kanban-column {
    min-width: unset;
  }
}
`;
document.head.appendChild(style);

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
