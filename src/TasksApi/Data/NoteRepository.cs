using Core;
using Core.RepositoryPattern;
using Microsoft.EntityFrameworkCore;
using TasksApi.Models;

namespace TasksApi.Data;

public interface INoteRepository : IRepository<Note>
{
    Task<List<Note>> GetNotesByTaskId(int taskId);
    Task<List<Note>> GetNotesByUserId(int userId);
}
public class NoteRepository:Repository<Note>,INoteRepository
{
    private AppDbContext AppDbContext => (AppDbContext)_context;
    public NoteRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Note>> GetNotesByTaskId(int taskId)
    {
        return await AppDbContext.Notes.Where(n => n.TaskId == taskId)
            .Include(n => n.Task)
            .Include(n => n.CreatorUser)
            .ToListAsync();
    }

    public async Task<List<Note>> GetNotesByUserId(int userId)
    {
        return await AppDbContext.Notes.Where(n => n.CreatorUserId == userId)
            .Include(n => n.Task)
            .Include(n => n.CreatorUser)
            .ToListAsync();
    }
}