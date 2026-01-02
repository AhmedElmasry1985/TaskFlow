using TasksApi.DTOs;

namespace TasksApi.Services.GrpcClient;

public interface IUsersClient
{
    Task<GetAllUsersProcedureDtoResponse> GetAllUsers(GetAllUsersProcedureDtoRequest request);
}