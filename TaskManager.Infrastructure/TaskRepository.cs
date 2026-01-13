using Microsoft.EntityFrameworkCore;
using TaskManager.Application;
using TaskManager.Domain;

namespace TaskManager.Infrastructure;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;

    public TaskRepository(TaskDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<IEnumerable<TaskManager.Domain.Task>> GetAllAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async System.Threading.Tasks.Task<TaskManager.Domain.Task?> GetByIdAsync(int id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async System.Threading.Tasks.Task<TaskManager.Domain.Task> AddAsync(TaskManager.Domain.Task task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async System.Threading.Tasks.Task UpdateAsync(TaskManager.Domain.Task task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}