using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UpskillingApi.Data;
using UpskillingApi.Middleware;
using UpskillingApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// Controllers
// ==============================
builder.Services.AddControllers();

// ==============================
// API Versioning
// ==============================
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Explorer para o Swagger versionado
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // v1, v2...
    options.SubstituteApiVersionInUrl = true;
});

// ==============================
// Banco Oracle
// ==============================
var connectionString = builder.Configuration.GetConnectionString("conn");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString));

// ==============================
// Health Check
// ==============================
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database");

// ==============================
// Cache e Serviços
// ==============================
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IDataCacheService, DataCacheService>();

// ==============================
// JWT Authentication
// ==============================
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection.GetValue<string>("Key")
    ?? "SUA_CHAVE_SECRETA_DEV_AQUI_ALTERE_PARA_PROJETO_REAL";
var issuer = jwtSection.GetValue<string>("Issuer") ?? "upskilling-api";
var audience = jwtSection.GetValue<string>("Audience") ?? "upskilling-api-clients";

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = key,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ==============================
// Swagger (com JWT + versionado)
// ==============================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Adiciona suporte ao botão Authorize (JWT)
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Informe o token JWT no formato: Bearer {seuToken}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Importante para Swagger por versão
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// ==============================
// Build
// ==============================
var app = builder.Build();

// ==============================
// Middleware de Logging
// ==============================
app.UseMiddleware<RequestLoggingMiddleware>();

// ==============================
// Swagger + UI
// ==============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"Upskilling API {description.GroupName.ToUpper()}");
        }
    });
}

// ==============================
// Pipeline
// ==============================
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
