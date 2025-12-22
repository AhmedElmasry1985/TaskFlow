using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RabbitMQ.Client;

namespace UsersApi.Services.MessageBus;

public class RabbitMQClient : IAsyncDisposable, IMessageBusClient
{
    private static readonly string ExchangeName = "taskflow";
    private readonly IConfiguration _configuration;
    private bool _isInitialized = false;
    IConnection _connection;
    IChannel _channel;
    private SemaphoreSlim _semaphore = new(1, 1);

    public RabbitMQClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private async Task Initialize()
    {
        if (_isInitialized) return;
        await _semaphore.WaitAsync();
        try
        {
            if (_isInitialized) return;
            var rabbitMQSettings = _configuration.GetSection("RabbitMQ");
            var factory = new ConnectionFactory
            {
                HostName = rabbitMQSettings.GetSection("Host").Value,
                UserName = rabbitMQSettings.GetSection("UserName").Value,
                Password = rabbitMQSettings.GetSection("Password").Value
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout);
            _isInitialized = true;
            Console.WriteLine("--> RabbitMQ connected successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> RabbitMQ connection failed: {ex.Message}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task PublishMessage(string message)
    {
        await Initialize();
        if (_connection.IsOpen && _channel.IsOpen)
        {
            await _channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: string.Empty,
                mandatory: false,
                body: Encoding.UTF8.GetBytes(message));
            Console.WriteLine($"--> Message published: {message}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("--> Disposing RabbitMQ connection");
        if (_channel is { IsOpen: true }) await _channel.CloseAsync();
        if (_connection is { IsOpen: true }) await _connection.CloseAsync();
        _channel.Dispose();
        _connection.Dispose();
        _semaphore.Dispose();
    }
}