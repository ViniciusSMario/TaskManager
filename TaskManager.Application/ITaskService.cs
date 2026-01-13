using TaskManager.Domain;

namespace TaskManager.Application;

/// <summary>
/// Interface de serviço contendo a lógica de aplicação para tarefas
/// </summary>
public interface ITaskService
{
    System.Threading.Tasks.Task<IEnumerable<TaskManager.Domain.Task>> GetAllTasksAsync();
    System.Threading.Tasks.Task<TaskManager.Domain.Task?> GetTaskByIdAsync(int id);
    System.Threading.Tasks.Task<TaskManager.Domain.Task> CreateTaskAsync(TaskManager.Domain.Task task);
    System.Threading.Tasks.Task UpdateTaskAsync(TaskManager.Domain.Task task);
    System.Threading.Tasks.Task DeleteTaskAsync(int id);
    System.Threading.Tasks.Task<bool> ValidateTaskAsync(TaskManager.Domain.Task task, out string errorMessage);
}