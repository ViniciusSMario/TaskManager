import { StatusLabels } from '../services/taskService.js';

export function createTaskFilter(onFilterChange) {
  const filter = document.createElement('div');
  filter.className = 'task-filter';
  
  filter.innerHTML = `
    <div class="filter-buttons">
      <button class="filter-btn active" data-status="all">Todas</button>
      <button class="filter-btn" data-status="0">Pendentes</button>
      <button class="filter-btn" data-status="1">Em Progresso</button>
      <button class="filter-btn" data-status="2">Concluídas</button>
    </div>
  `;

  const buttons = filter.querySelectorAll('.filter-btn');
  buttons.forEach(btn => {
    btn.addEventListener('click', () => {
      buttons.forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      const status = btn.dataset.status === 'all' ? null : parseInt(btn.dataset.status);
      onFilterChange(status);
    });
  });

  return filter;
}
