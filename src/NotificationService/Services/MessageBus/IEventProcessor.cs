namespace NotificationService.Services.MessageBus;

public interface IEventProcessor
{
    Task<bool> ProcessEvent(string message);
}