using Carter;
using LIMS.Service.LaboratoryOperations.API;
using LIMS.Service.LaboratoryOperations.API.BackgroundServices;
using NoStringEvaluating.Extensions.Microsoft.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddNoStringEvaluator();


builder.Services.AddSwaggerGen();

builder.Services.AddApi(builder.Configuration);
builder.Services.AddHostedService<BackgroundOperationWorker>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.Run();
