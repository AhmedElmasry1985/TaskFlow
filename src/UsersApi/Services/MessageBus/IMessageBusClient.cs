namespace UsersApi.Services.MessageBus;

public interface IMessageBusClient
{
    Task PublishMessage(string message);
}