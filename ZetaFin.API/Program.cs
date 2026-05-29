using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ZetaFin.Application.Interfaces;
using ZetaFin.Application.Services;
using ZetaFin.Domain.Interfaces;
using ZetaFin.Infrastructure.Services;
using ZetaFin.Persistence;
using ZetaFin.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ===================================
// CONFIGURAÇÃO DE LOGGING
// ===================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (builder.Environment.IsProduction())
{
    builder.Logging.AddEventSourceLogger();
}

// ===================================
// CONFIGURAÇÃO DO BANCO DE DADOS
// ===================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(60);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// ===================================
// DEPENDENCY INJECTION
// ===================================

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IUserWhatsAppRepository, UserWhatsAppRepository>();
builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<ExpenseCategories, DepositRepository>();
builder.Services.AddScoped<IUserGoalRepository, UserGoalRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IPreRegistrationRepository, PreRegistrationRepository>(); // ← NOVO

// Authentication & Security Services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Business Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddScoped<IUserGoalService, UserGoalService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IWhatsAppAuthService, WhatsAppAuthService>();
builder.Services.AddScoped<IPreRegistrationService, PreRegistrationService>(); // ← NOVO

// Infrastructure Services
var storagePath = builder.Configuration["Storage:BasePath"] ?? "storage";
var storageUrl = builder.Configuration["Storage:BaseUrl"] ?? "http://localhost:8080/storage";
builder.Services.AddScoped<IFileStorageService>(sp =>
    new LocalFileStorageService(storagePath, storageUrl));
builder.Services.AddScoped<IOcrService, MockOcrService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ===================================
// JWT AUTHENTICATION
// ===================================
var jwtSecret = builder.Configuration["Jwt:Secret"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT Secret não configurado.");
}

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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ===================================
// CONTROLLERS & CORS
// ===================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ===================================
// MIDDLEWARE PIPELINE
// ===================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => new
{
    Application = "ZetaFin API",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Status = "Running"
});

// ===================================
// START APPLICATION
// ===================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Logger.LogInformation($"ZetaFin API iniciando na porta {port}...");
app.Logger.LogInformation($"Ambiente: {app.Environment.EnvironmentName}");
app.Logger.LogInformation($"Swagger: {(app.Environment.IsDevelopment() ? "http://localhost:8080/swagger" : "Desabilitado")}");

app.Run();