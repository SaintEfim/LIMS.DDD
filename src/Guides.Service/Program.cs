using Library.Broker.Messages;
using Carter;
using Guides.Service.Commands;
using Guides.Service.Persistence;
using Microsoft.EntityFrameworkCore;
using Library.Broker.RabbitMq.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ServiceDB")));

builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

builder.Services
    .AddRabbitMq(options =>
    {
        options.HostName = "localhost";
        options.Port = 5672;
        options.UserName = "guest";
        options.Password = "guest";
    }, "guid-service")
    .AddMessage<UnitCreatedMessage>()
    .AddOutbox();

builder.Services.AddScoped<UnitCreatedDomainEventHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.Run();
