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
            ${task
              ? Object.entries(StatusLabels).map(([value, label]) => `
                  <option value="${value}" ${task && task.status == value ? 'selected' : ''}>
                    ${label}
                  </option>
                `).join('')
              : Object.entries(StatusLabels)
                  .filter(([value]) => value == 0 || value == 1)
                  .map(([value, label]) => `
                    <option value="${value}">
                      ${label}
                    </option>
                  `).join('')
            }
          </select>
          ${task && task.status === 2 ? '<small class="status-info">Tarefa concluída não pode ter o status alterado</small>' : ''}
        </div>

        ${task && task.status === 2 ? `
        <div class="form-group" id="dataConclusao-group">
          <label for="dataConclusao">Data de Conclusão</label>
          <input 
            type="datetime-local" 
            id="dataConclusao" 
            name="dataConclusao"
            value="${task && task.dataConclusao ? (() => { const d = new Date(task.dataConclusao); const year = d.getFullYear(); const month = String(d.getMonth() + 1).padStart(2, '0'); const day = String(d.getDate()).padStart(2, '0'); const hours = String(d.getHours()).padStart(2, '0'); const minutes = String(d.getMinutes()).padStart(2, '0'); return `${year}-${month}-${day}T${hours}:${minutes}`; })() : ''}"
          />
        </div>
        ` : ''}

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

  // Função para mostrar/ocultar campo de dataConclusao
  function updateDataConclusaoField() {
    let dataConclusaoGroup = formContainer.querySelector('#dataConclusao-group');
    if (statusSelect.value === '2') {
      if (!dataConclusaoGroup) {
        // Adiciona campo
        const formActions = formContainer.querySelector('.form-actions');
        const div = document.createElement('div');
        div.className = 'form-group';
        div.id = 'dataConclusao-group';
        div.innerHTML = `
          <label for="dataConclusao">Data de Conclusão</label>
          <input type="datetime-local" id="dataConclusao" name="dataConclusao" />
        `;
        formActions.parentNode.insertBefore(div, formActions);
        // Preenche valor se necessário
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');
        div.querySelector('#dataConclusao').value = `${year}-${month}-${day}T${hours}:${minutes}`;
      }
    } else {
      if (dataConclusaoGroup) {
        dataConclusaoGroup.remove();
      }
    }
  }

  statusSelect.addEventListener('change', updateDataConclusaoField);

  // Se for nova tarefa, não mostra campo; se for edição e status=2, já mostra
  // (já renderizado pelo template)

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    const formData = new FormData(form);
    const status = parseInt(formData.get('status'));
    let dataConclusao = null;
    if (status === 2) {
      dataConclusao = formData.get('dataConclusao') || null;
    }
    const taskData = {
      titulo: formData.get('titulo'),
      descricao: formData.get('descricao') || null,
      status,
      dataConclusao,
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
