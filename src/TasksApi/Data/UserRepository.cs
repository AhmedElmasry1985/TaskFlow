using Core;
using Microsoft.EntityFrameworkCore;
using TasksApi.Models;

namespace TasksApi.Data;

public interface IUserRepository:IRepository<User>
{
    Task<User?> GetUserByExternalId(int externalId);
    Task<bool> Save(); //No need to Unit of Work pattern here
}
public class UserRepository:Repository<User>,IUserRepository
{
    private AppDbContext AppDbContext => (AppDbContext)_context;

    public UserRepository(AppDbContext context) : base(context)
    {
        
    }

    public async Task<User?> GetUserByExternalId(int externalId)
    {
        return await AppDbContext.Users.FirstOrDefaultAsync(u => u.ExternalId == externalId);
    }

    public async Task<bool> Save()
    {
        return await AppDbContext.SaveChangesAsync() > 0;
    }
}