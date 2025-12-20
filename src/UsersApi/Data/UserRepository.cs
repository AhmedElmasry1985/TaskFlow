using Core;
using Microsoft.EntityFrameworkCore;
using UsersApi.Models;

namespace UsersApi.Data;

public class UserRepository:Repository<User>,IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
    private AppDbContext AppDbContext => (AppDbContext)_context;

   
    public async Task<User?> GetUserByUsername(string username)
    {
        return
           await AppDbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> Save()
    {
        return await AppDbContext.SaveChangesAsync() > 0;
    }
}