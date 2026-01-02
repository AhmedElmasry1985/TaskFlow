using Grpc.Net.Client;
using TasksApi.DTOs;
using UsersApi;

namespace TasksApi.Services.GrpcClient;

public class UsersClient(IConfiguration configuration):IUsersClient
{
    private readonly string? _usersServiceGrpcUrl = configuration.GetSection("UsersServiceGrpcUrl").Value;

    public async Task<GetAllUsersProcedureDtoResponse> GetAllUsers(GetAllUsersProcedureDtoRequest request)
    {
        var response = new GetAllUsersProcedureDtoResponse();
        try
        {
            var channel = GrpcChannel.ForAddress(_usersServiceGrpcUrl);
            var client = new GrpcUsersService.GrpcUsersServiceClient(channel);
            var grpcResult = await client.GetAllUsersAsync(new GetAllUsersRequest());
            response.Result.IsSuccess = grpcResult.IsSuccessful;
            response.Result.Message = grpcResult.Message;
            response.Users.AddRange(grpcResult.Users.Select(user => new UsersProcedureDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Role = user.Role,
                Username = user.Username
            }));
        }
        catch (Exception e)
        {
            response.Result.IsSuccess = false;
            response.Result.Message = e.Message;
        }
        return response;
    }
}