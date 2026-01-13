using TaskManager.Application;
using TaskManager.Domain;

namespace TaskManager.Infrastructure;

/// <summary>
/// Implementação da lógica de aplicação para gerenciamento de tarefas
/// </summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async System.Threading.Tasks.Task<IEnumerable<TaskManager.Domain.Task>> GetAllTasksAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async System.Threading.Tasks.Task<TaskManager.Domain.Task?> GetTaskByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async System.Threading.Tasks.Task<TaskManager.Domain.Task> CreateTaskAsync(TaskManager.Domain.Task task)
    {
        // Lógica de negócio: definir data de criação
        if (task.DataCriacao == default)
        {
            task.DataCriacao = DateTime.Now;
        }

        // Validação de negócio
        if (!await ValidateTaskAsync(task, out string errorMessage))
        {
            throw new InvalidOperationException(errorMessage);
        }

        return await _repository.AddAsync(task);
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(TaskManager.Domain.Task task)
    {
        var existingTask = await _repository.GetByIdAsync(task.Id);
        if (existingTask == null)
        {
            throw new KeyNotFoundException($"Tarefa com ID {task.Id} não encontrada");
        }

        // Validação de negócio
        if (!await ValidateTaskAsync(task, out string errorMessage))
        {
            throw new InvalidOperationException(errorMessage);
        }

        // Lógica de negócio: se status mudou para Concluída, definir data de conclusão
        if (task.Status == TaskManager.Domain.TaskStatus.Concluida && existingTask.Status != TaskManager.Domain.TaskStatus.Concluida)
        {
            if (!task.DataConclusao.HasValue)
            {
                task.DataConclusao = DateTime.Now;
            }
        }

        await _repository.UpdateAsync(task);
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(int id)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task == null)
        {
            throw new KeyNotFoundException($"Tarefa com ID {id} não encontrada");
        }

        await _repository.DeleteAsync(id);
    }

    public System.Threading.Tasks.Task<bool> ValidateTaskAsync(TaskManager.Domain.Task task, out string errorMessage)
    {
        // Validação de título
        if (string.IsNullOrWhiteSpace(task.Titulo))
        {
            errorMessage = "O título é obrigatório";
            return System.Threading.Tasks.Task.FromResult(false);
        }

        if (task.Titulo.Length > 100)
        {
            errorMessage = "O título deve ter no máximo 100 caracteres";
            return System.Threading.Tasks.Task.FromResult(false);
        }

        // Validação de datas
        if (!task.IsValid(out errorMessage))
        {
            return System.Threading.Tasks.Task.FromResult(false);
        }

        errorMessage = string.Empty;
        return System.Threading.Tasks.Task.FromResult(true);
    }
}