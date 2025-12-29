namespace TasksApi.Services.MessageBus;

public interface IEventProcessor
{
     Task<bool> ProcessEvent(string message);
}