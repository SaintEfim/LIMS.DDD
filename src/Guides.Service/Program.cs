using Carter;
using Guides.Service.Persistence;
using Guides.Messages;
using Microsoft.EntityFrameworkCore;
using RabbitMq.Library.QuickStart.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRabbitMq(x =>
{
    x.HostName = "localhost";
    x.Port = 5672;
    x.UserName = "guest";
    x.Password = "guest";
}, typeof(UnitCreatedMessage).Assembly).Build();

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
