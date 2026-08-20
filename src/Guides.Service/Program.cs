using Carter;
using Guides.Messages;
using Guides.Service.Persistence;
using Microsoft.EntityFrameworkCore;
using RabbitMq.Library.QuickStart.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddRabbitMq(options =>
    {
        options.HostName = "localhost";
        options.Port = 5672;
        options.UserName = "guest";
        options.Password = "guest";
    })
    .AddMessage<UnitCreatedMessage>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ServiceDB")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.Run();
