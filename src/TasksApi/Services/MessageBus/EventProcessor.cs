using System.Text.Json;
using AutoMapper;
using Core;
using TasksApi.Data;
using TasksApi.DTOs;
using TasksApi.Models;

namespace TasksApi.Services.MessageBus;

public class EventProcessor(IMapper mapper, IServiceScopeFactory serviceScopeFactory):IEventProcessor
{
   
    public async Task<bool> ProcessEvent(string message)
    {
        var @event = JsonSerializer.Deserialize<MessageDto>(message);
        switch (@event?.EventName)
        {
            case Strings.UserRegisteredEvent:
                return await AddUser(message);
        }
        return true;
    }

    private async Task<bool> AddUser(string message)
    {
        var userDto = JsonSerializer.Deserialize<RegisterUserDtoRequest>(message);
        var user = mapper.Map<User>(userDto);
        using var scope = serviceScopeFactory.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        try
        {
            var existingUser = await userRepository.GetUserByExternalId(user.ExternalId);
            if (existingUser != null)
            {
                Console.WriteLine($"User with Id: {user.ExternalId} already exists");
                return false;
            }
            
            await userRepository.Add(user);
            var result = await userRepository.Save();
            if(result)
                Console.WriteLine($"User with Id: {user.ExternalId} added");
            else Console.WriteLine($"User with Id: {user.ExternalId} not added");
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to add User with Id: {user.ExternalId} - {e.Message}");
            return false;
        }
    }
}