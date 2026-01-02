using Core;
using Microsoft.EntityFrameworkCore;
using UsersApi.Data;
using UsersApi.Migrations;
using UsersApi.Services.Auth;
using UsersApi.Services.GrpcServer;
using UsersApi.Services.MessageBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<Jwt>();
builder.Services.AddGrpc();
builder.Services.AddSingleton<IMessageBusClient, RabbitMQClient>();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();
app.Migrate();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
app.MapGrpcService<UsersService>();
app.MapGet("/protos/users.proto", () => File.ReadAllText("Protos/Users.proto"));
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();