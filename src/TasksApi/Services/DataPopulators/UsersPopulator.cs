using TasksApi.Data;
using TasksApi.DTOs;
using TasksApi.Models;
using TasksApi.Services.GrpcClient;
using Task = System.Threading.Tasks.Task;

namespace TasksApi.Services.DataPopulators;

public static class UsersPopulator
{
    public static async Task PopulateUsers(this IApplicationBuilder app)
    {
        using var scope =app.ApplicationServices.CreateScope();
        var usersClient = scope.ServiceProvider.GetRequiredService<IUsersClient>();
        var usersResponse = await usersClient.GetAllUsers(new GetAllUsersProcedureDtoRequest());
        if (usersResponse.Result.IsSuccess)
        {
            Console.WriteLine("--> Users retrieved successfully from UsersService");
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var existingUsersIds = (await userRepository.FindByPredicate()).Select(u => u.ExternalId).ToHashSet();
            var newUsers = usersResponse.Users.Where(u => !existingUsersIds.Contains(u.Id)).ToHashSet();
            if (newUsers.Count != 0)
            {
                Console.WriteLine($"--> Found {newUsers.Count} new users to populate");
                foreach (var user in newUsers)
                {
                    await userRepository.Add(new User
                    {
                        ExternalId = user.Id,
                        FullName = user.FullName,
                        Username = user.Username,
                    });
                }
                var result = await userRepository.Save();
                if(result)
                    Console.WriteLine($"--> {newUsers.Count} users populated successfully");
                else Console.WriteLine("--> Users not populated");
            }
            else Console.WriteLine("--> No new users to populate");

        }
        else Console.WriteLine($"--> Failed to retrieve users from UsersService, Error: {usersResponse.Result.Message}");
    }

}