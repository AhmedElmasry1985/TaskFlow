using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Services.MessageBus;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<RabbitMQSubscriber>();
var app = builder.Build();
Console.WriteLine("--> NotificationService starting...");
await app.RunAsync();