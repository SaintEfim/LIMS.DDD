using Carter;
using LIMS.DDD.Service.Application;
using LIMS.DDD.Service.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.Run();
