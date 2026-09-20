using Carter;
using LIMS.Service.Methodologies.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NoStringEvaluating.Extensions.Microsoft.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddNoStringEvaluator();

var keycloak = builder.Configuration.GetRequiredSection("Keycloak");
var keycloakAuthority = keycloak.GetValue<string>("Authority")!;
var keycloakSwaggerClientId = keycloak.GetValue<string>("SwaggerClientId")!;

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string> { ["openid"] = "OpenID Connect" }
            }
        }
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Keycloak", document, null)] = ["openid"]
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthority;
        options.Audience = keycloak["Audience"];
        options.RequireHttpsMetadata = keycloak.GetValue("RequireHttpsMetadata", true);
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "sub",
            ValidIssuer = keycloakAuthority
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId(keycloakSwaggerClientId);
        options.OAuthUsePkce();
        options.OAuthScopes("openid");
    });
}

app.MapCarter();
app.Run();
