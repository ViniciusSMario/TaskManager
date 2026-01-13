import { TaskStatus, StatusLabels } from '../services/taskService.js';

export function createTaskForm(task = null, onSubmit, onCancel) {
  const formContainer = document.createElement('div');
  formContainer.className = 'modal-overlay';
  
  formContainer.innerHTML = `
    <div class="modal-content">
      <div class="modal-header">
        <h2>${task ? 'Editar Tarefa' : 'Nova Tarefa'}</h2>
        <button class="btn-close" type="button">✕</button>
      </div>
      <form id="task-form">
        <div class="form-group">
          <label for="titulo">Título *</label>
          <input 
            type="text" 
            id="titulo" 
            name="titulo" 
            required 
            maxlength="100"
            value="${task ? task.titulo : ''}"
            placeholder="Digite o título da tarefa"
          />
        </div>

        <div class="form-group">
          <label for="descricao">Descrição</label>
          <textarea 
            id="descricao" 
            name="descricao" 
            rows="4"
            placeholder="Digite uma descrição (opcional)"
          >${task ? task.descricao || '' : ''}</textarea>
        </div>

        <div class="form-group">
          <label for="status">Status *</label>
          <select id="status" name="status" required ${task && task.status === 2 ? 'disabled' : ''}>
            ${Object.entries(StatusLabels).map(([value, label]) => `
              <option value="${value}" ${task && task.status == value ? 'selected' : ''}>
                ${label}
              </option>
            `).join('')}
          </select>
          ${task && task.status === 2 ? '<small class="status-info">Tarefa concluída não pode ter o status alterado</small>' : ''}
        </div>

        <div class="form-group">
          <label for="dataConclusao">Data de Conclusão</label>
          <input 
            type="datetime-local" 
            id="dataConclusao" 
            name="dataConclusao"
            value="${task && task.dataConclusao ? new Date(task.dataConclusao).toISOString().slice(0, 16) : ''}"
          />
        </div>

        <div class="form-actions">
          <button type="button" class="btn btn-secondary" id="btn-cancel">Cancelar</button>
          <button type="submit" class="btn btn-primary">${task ? 'Atualizar' : 'Criar'}</button>
        </div>
      </form>
    </div>
  `;

  const form = formContainer.querySelector('#task-form');
  const btnClose = formContainer.querySelector('.btn-close');
  const btnCancel = formContainer.querySelector('#btn-cancel');
  const statusSelect = formContainer.querySelector('#status');
  const dataConclusaoInput = formContainer.querySelector('#dataConclusao');

  statusSelect.addEventListener('change', (e) => {
    if (e.target.value === '2') {
      if (!dataConclusaoInput.value) {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');
        dataConclusaoInput.value = `${year}-${month}-${day}T${hours}:${minutes}`;
      }
    } else {
      dataConclusaoInput.value = '';
    }
  });

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const formData = new FormData(form);
    const taskData = {
      titulo: formData.get('titulo'),
      descricao: formData.get('descricao') || null,
      status: parseInt(formData.get('status')),
      dataConclusao: formData.get('dataConclusao') || null,
    };

    await onSubmit(taskData);
    formContainer.remove();
  });

  const closeModal = () => {
    formContainer.remove();
    if (onCancel) onCancel();
  };

  btnClose.addEventListener('click', closeModal);
  btnCancel.addEventListener('click', closeModal);
  formContainer.addEventListener('click', (e) => {
    if (e.target === formContainer) closeModal();
  });

  return formContainer;
}
