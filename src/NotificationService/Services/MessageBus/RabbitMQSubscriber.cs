using System.Text;
using Core;
using Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Services.MessageBus;

public class RabbitMQSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private bool _isInitialized = false;
    private IConnection _connection;
    private IChannel _channel;
    private SemaphoreSlim _semaphore = new(1, 1);
    private string _queueName;

    public RabbitMQSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;
    }

    private async Task Initialize(CancellationToken cancellationToken = default)
    {
        if (_isInitialized) return;
        await _semaphore.WaitAsync(cancellationToken);
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
            await Retry.ExecuteWithExponentialBackoffAsync(
                async () =>
                {
                    _connection = await factory.CreateConnectionAsync(cancellationToken);
                    _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
                    await _channel.ExchangeDeclareAsync(Strings.ExchangeName, ExchangeType.Fanout, cancellationToken: cancellationToken);
                
                    var args = new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", "taskflow-dlx" }
                    };
                    _queueName = await _channel.QueueDeclareAsync(arguments: args, cancellationToken: cancellationToken);
                    await _channel.QueueBindAsync(_queueName, Strings.ExchangeName, string.Empty, cancellationToken: cancellationToken);
                
                    return true;
                },
                maxRetries: 10,
                initialDelayMilliseconds: 2000,
                maxDelayMilliseconds: 30000,
                onRetry: (attempt, ex) => 
                    Console.WriteLine($"--> RabbitMQ connection attempt {attempt} failed: {ex.Message}"),
                cancellationToken: cancellationToken
            );
            Console.WriteLine("--> RabbitMQ Subscriber connected successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> RabbitMQ Subscriber connection failed: {ex.Message}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        await Initialize(stoppingToken);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (_, e) =>
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await _eventProcessor.ProcessEvent(message);
        };
        
        await _channel.BasicConsumeAsync(_queueName, true, consumer, cancellationToken: stoppingToken);
        Console.WriteLine("--> NotificationService: Listening for events...");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("--> RabbitMQ Subscriber stopped");
        if(_channel is {IsOpen:true}) await _channel.CloseAsync(cancellationToken);
        if(_connection is {IsOpen:true}) await _connection.CloseAsync(cancellationToken);
        _channel?.Dispose();
        _connection?.Dispose();
        _semaphore?.Dispose();
    }
}