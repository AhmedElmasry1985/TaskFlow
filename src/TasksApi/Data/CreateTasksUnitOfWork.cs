namespace TasksApi.Data;

public interface ICreateTasksUnitOfWork
{
    ITaskRepository TaskRepository { get; }
    INoteRepository NoteRepository { get; }
    Task<bool> SaveChangesAsync();
}

public class CreateTasksUnitOfWork : ICreateTasksUnitOfWork
{
    public ITaskRepository TaskRepository { get; }
    public INoteRepository NoteRepository { get; }
    private readonly AppDbContext _appDbContext;

    public CreateTasksUnitOfWork(AppDbContext appDbContext, ITaskRepository taskRepository, INoteRepository noteRepository)
    {
        _appDbContext = appDbContext;
        TaskRepository = taskRepository;
        NoteRepository = noteRepository;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _appDbContext.SaveChangesAsync() > 0;
    }
}