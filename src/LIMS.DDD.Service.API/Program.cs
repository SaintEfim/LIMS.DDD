using Carter;
using LIMS.DDD.Service.API;
using NoStringEvaluating.Extensions.Microsoft.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddNoStringEvaluator();

builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.Run();
