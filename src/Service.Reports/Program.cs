using Service.Reports;
using Service.Reports.Client.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<OrderReportGenerator>();

var keycloak = builder.Configuration.GetRequiredSection("Keycloak");
var keycloakAuthority = keycloak.GetValue<string>("Authority")!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthority;
        options.Audience = keycloak["Audience"];
        options.RequireHttpsMetadata = keycloak.GetValue("RequireHttpsMetadata", true);
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "user_id",
            ValidIssuer = keycloakAuthority
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

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
    .RequireAuthorization()
    .Produces(StatusCodes.Status200OK, contentType: "application/pdf")
    .Produces(StatusCodes.Status400BadRequest);

app.Run();
