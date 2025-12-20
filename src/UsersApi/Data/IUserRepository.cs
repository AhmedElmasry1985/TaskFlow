using Core;
using UsersApi.Models;

namespace UsersApi.Data;

public interface IUserRepository: IRepository<User>
{
    Task<User?> GetUserByUsername(string username);
    Task<bool> Save(); //No need to Unit of Work pattern here
}