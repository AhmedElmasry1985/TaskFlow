using Grpc.Core;
using UsersApi.Data;

namespace UsersApi.Services.GrpcServer;

public class UsersService(IUserRepository userRepository):GrpcUsersService.GrpcUsersServiceBase
{
    public override async Task<GetAllUsersResponse> GetAllUsers(GetAllUsersRequest request, ServerCallContext context)
    {
        var response = new GetAllUsersResponse();
        try
        {
            var users = await userRepository.FindByPredicate();
            response.Users.AddRange(users.Select(user => new GrpcUserModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Role = user.Role,
                Username = user.Username
            }));
            response.Message = "Users retrieved successfully";
            response.IsSuccessful = true;
        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.IsSuccessful = false;
        }
        return response;
    }
}