namespace TaskManager.Application;

public interface ITaskRepository
{
    System.Threading.Tasks.Task<IEnumerable<TaskManager.Domain.Task>> GetAllAsync();
    System.Threading.Tasks.Task<TaskManager.Domain.Task?> GetByIdAsync(int id);
    System.Threading.Tasks.Task<TaskManager.Domain.Task> AddAsync(TaskManager.Domain.Task task);
    System.Threading.Tasks.Task UpdateAsync(TaskManager.Domain.Task task);
    System.Threading.Tasks.Task DeleteAsync(int id);
    System.Threading.Tasks.Task<bool> ExistsByTitleAsync(string titulo);
}