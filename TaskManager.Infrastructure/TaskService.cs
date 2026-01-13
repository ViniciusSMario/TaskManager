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

        // Validação de negócio para criação (inclui verificação de título único)
        var validationResult = await ValidateTaskForCreationAsync(task);
        if (!validationResult.isValid)
        {
            throw new InvalidOperationException(validationResult.errorMessage);
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

        // Validação de negócio: não permitir alterar status de tarefa concluída
        if (existingTask.Status == TaskManager.Domain.TaskStatus.Concluida && task.Status != TaskManager.Domain.TaskStatus.Concluida)
        {
            throw new InvalidOperationException("Não é possível alterar o status de uma tarefa concluída");
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

    public async System.Threading.Tasks.Task<(bool isValid, string errorMessage)> ValidateTaskForCreationAsync(TaskManager.Domain.Task task)
    {
        // Validações básicas
        if (!await ValidateTaskAsync(task, out string errorMessage))
        {
            return (false, errorMessage);
        }

        // Verificar se já existe uma tarefa com o mesmo título
        if (await _repository.ExistsByTitleAsync(task.Titulo))
        {
            return (false, "Já existe uma tarefa com este título");
        }

        return (true, string.Empty);
    }
}