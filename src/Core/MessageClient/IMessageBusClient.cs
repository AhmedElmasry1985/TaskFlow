namespace Core.MessageClient;

public interface IMessageBusClient
{
    Task PublishMessage(string message);
}