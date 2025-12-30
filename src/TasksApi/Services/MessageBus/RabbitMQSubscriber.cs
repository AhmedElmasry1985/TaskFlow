using System.Text;
using Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TasksApi.Services.MessageBus;

public class RabbitMQSubscriber(IConfiguration configuration, IEventProcessor eventProcessor) : BackgroundService
{
    private bool _isInitialized = false;
    IConnection _connection;
    IChannel _channel;
    private SemaphoreSlim _semaphore = new(1, 1);
    private string _queueName;

    private async Task Initialize()
    {
        if (_isInitialized) return;
        await _semaphore.WaitAsync();
        try
        {
            if (_isInitialized) return;
            var rabbitMQSettings = configuration.GetSection("RabbitMQ");
            var factory = new ConnectionFactory
            {
                HostName = rabbitMQSettings.GetSection("Host").Value,
                UserName = rabbitMQSettings.GetSection("UserName").Value,
                Password = rabbitMQSettings.GetSection("Password").Value
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(Strings.ExchangeName, ExchangeType.Fanout);
            //todo: Dead Letter Queue
            var args = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "taskflow-dlx" }
            };
            _queueName = await _channel.QueueDeclareAsync(arguments: args);            await _channel.QueueBindAsync(_queueName, Strings.ExchangeName, string.Empty);
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        await Initialize();
        var consumer = new AsyncEventingBasicConsumer(_channel);
        //todo: Manual Acknowledgment for Reliability:
        consumer.ReceivedAsync += async (_, e) =>
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await eventProcessor.ProcessEvent(message);
            //todo: Manual Acknowledgment for Reliability:
            //put ProcessEvent in try catch block
            //if success, call _channel.BasicAckAsync, else call _channel.BasicNackAsync
        };
        await _channel.BasicConsumeAsync(_queueName, true, consumer, cancellationToken: stoppingToken);
        Console.WriteLine("--> RabbitMQ consumer started");
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("--> RabbitMQ consumer stopped");
        if(_channel is {IsOpen:true}) await _channel.CloseAsync(cancellationToken);
        if(_connection is {IsOpen:true}) await _connection.CloseAsync(cancellationToken);
        _channel?.Dispose();
        _connection?.Dispose();
        _semaphore?.Dispose();
    }
}