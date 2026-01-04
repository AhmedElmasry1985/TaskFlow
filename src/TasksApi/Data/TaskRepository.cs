using Core;
using Core.RepositoryPattern;
using Microsoft.EntityFrameworkCore;
using Task = TasksApi.Models.Task;

namespace TasksApi.Data;


public interface ITaskRepository : IRepository<Task>
{
    Task<List<Task>> GetTasksCreatedByUser(int userId);
    Task<List<Task>> GetTasksAssignedToUser(int userId);
    Task<List<Task>> GetTasksByStatus(int statusId);
}

public class TaskRepository:Repository<Task>,ITaskRepository
{
    private AppDbContext AppDbContext => (AppDbContext)_context;
    public TaskRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Task>> GetTasksCreatedByUser(int userId)
    {
        return await AppDbContext.Tasks.Where(t => t.CreatorUserId == userId)
            .Include(t => t.Notes)
            .ThenInclude(n=>n.CreatorUser)
            .Include(t => t.Status)
            .Include(t => t.CreatorUser)
            .Include(t => t.AssignedUser)
            .ToListAsync();
    }

    public async Task<List<Task>> GetTasksAssignedToUser(int userId)
    {
        return await AppDbContext.Tasks.Where(t => t.AssignedUserId == userId)
            .Include(t => t.Notes)
            .ThenInclude(n=>n.CreatorUser)
            .Include(t => t.Status)
            .Include(t => t.CreatorUser)
            .Include(t => t.AssignedUser)
            .ToListAsync();
    }

    public async Task<List<Task>> GetTasksByStatus(int statusId)
    {
        return await AppDbContext.Tasks.Where(t => t.StatusId == statusId)
            .Include(t => t.Notes)
            .ThenInclude(n=>n.CreatorUser)
            .Include(t => t.Status)
            .Include(t => t.CreatorUser)
            .Include(t => t.AssignedUser)
            .ToListAsync();
    }
}