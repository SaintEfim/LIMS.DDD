using Service.Reports;
using Service.Reports.Client.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<OrderReportGenerator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/reports", (
        OrderReportRequest request,
        OrderReportGenerator generator) =>
    {
        if (request.Order is null || request.Order.Id == Guid.Empty) return Results.BadRequest("Order is required.");

        var pdf = generator.Generate(request);
        return Results.File(pdf, "application/pdf", $"order-{request.Order.Id}.pdf");
    })
    .WithName("GenerateReport")
    .Produces(StatusCodes.Status200OK, contentType: "application/pdf")
    .Produces(StatusCodes.Status400BadRequest);

app.Run();
