import { StatusLabels } from '../services/taskService.js';

export function createTaskItem(task, onEdit, onDelete) {
  const item = document.createElement('div');
  item.className = `task-item status-${task.status}`;
  item.innerHTML = `
    <div class="task-header">
      <h3>${task.titulo}</h3>
      <span class="task-status">${StatusLabels[task.status]}</span>
    </div>
    ${task.descricao ? `<p class="task-description">${task.descricao}</p>` : ''}
    <div class="task-dates">
      <span>📅 Criada: ${new Date(task.dataCriacao).toLocaleString('pt-BR')}</span>
      ${task.dataConclusao ? `<span>✅ Concluída: ${new Date(task.dataConclusao).toLocaleString('pt-BR')}</span>` : ''}
    </div>
    <div class="task-actions">
      <button class="btn-edit" data-id="${task.id}">✏️ Editar</button>
      <button class="btn-delete" data-id="${task.id}">🗑️ Deletar</button>
    </div>
  `;

  // Event listeners
  item.querySelector('.btn-edit').addEventListener('click', () => onEdit(task));
  item.querySelector('.btn-delete').addEventListener('click', () => onDelete(task.id));

  return item;
}
