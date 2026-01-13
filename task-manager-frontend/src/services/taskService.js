const API_URL = 'http://localhost:5194/api/tasks';

// Buscar todas as tarefas
export async function getAllTasks() {
  try {
    const response = await fetch(API_URL);
    if (!response.ok) throw new Error('Erro ao buscar tarefas');
    return await response.json();
  } catch (error) {
    console.error('Erro:', error);
    throw error;
  }
}

// Buscar tarefa por ID
export async function getTaskById(id) {
  try {
    const response = await fetch(`${API_URL}/${id}`);
    if (!response.ok) throw new Error('Tarefa não encontrada');
    return await response.json();
  } catch (error) {
    console.error('Erro:', error);
    throw error;
  }
}

// Criar nova tarefa
export async function createTask(task) {
  try {
    const response = await fetch(API_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(task),
    });
    if (!response.ok) {
      let errorMsg = 'Erro ao criar tarefa';
      try {
        const error = await response.json();
        if (error && error.errors) {
          if (typeof error.errors === 'string') errorMsg = error.errors;
          else if (Array.isArray(error.errors)) errorMsg = error.errors.join(' ');
          else if (typeof error.errors === 'object') {
            errorMsg = Object.values(error.errors).flat().join(' ');
          }
        } else if (error && error.error) {
          errorMsg = error.error;
        }
      } catch {}
      throw new Error(errorMsg);
    }
    return await response.json();
  } catch (error) {
    console.error('Erro:', error);
    throw error;
  }
}

// Atualizar tarefa
export async function updateTask(id, task) {
  try {
    const response = await fetch(`${API_URL}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(task),
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || 'Erro ao atualizar tarefa');
    }
    return true;
  } catch (error) {
    console.error('Erro:', error);
    throw error;
  }
}

// Deletar tarefa
export async function deleteTask(id) {
  try {
    const response = await fetch(`${API_URL}/${id}`, {
      method: 'DELETE',
    });
    if (!response.ok) throw new Error('Erro ao deletar tarefa');
    return true;
  } catch (error) {
    console.error('Erro:', error);
    throw error;
  }
}

// Status enum
export const TaskStatus = {
  Pendente: 0,
  EmProgresso: 1,
  Concluida: 2,
};

// Labels dos status
export const StatusLabels = {
  0: 'Pendente',
  1: 'Em Progresso',
  2: 'Concluída',
};
