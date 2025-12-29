using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<IDepositRepository, DepositRepository>();
builder.Services.AddScoped<IUserGoalRepository, UserGoalRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddScoped<IUserGoalService, UserGoalService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();

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

// ===================================
// HEALTH CHECKS
// ===================================
//builder.Services.AddHealthChecks().AddNpgSql(connectionString, name: "database", tags: new[] { "db", "sql" });

var app = builder.Build();

// ===================================
// EXECUTAR MIGRATIONS AUTOMATICAMENTE
// ===================================
//if (args.Contains("--migrate"))
//{
//    Console.WriteLine("Executando migrations...");

//    using var scope = app.Services.CreateScope();
//    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//    try
//    {
//        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

//        if (pendingMigrations.Any())
//        {
//            Console.WriteLine($"{pendingMigrations.Count()} migrations pendentes encontradas.");
//            await dbContext.Database.MigrateAsync();
//            Console.WriteLine("Migrations executadas com sucesso!");
//        }
//        else
//        {
//            Console.WriteLine("Banco de dados já está atualizado.");
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Erro ao executar migrations: {ex.Message}");
//        throw;
//    }

//    // Sair após migrations se for apenas para executá-las
//    if (args.Contains("--migrate-only"))
//    {
//        return;
//    }
//}
//else
//{
//    // Verificar conexão e aplicar migrations automaticamente no startup
//    using var scope = app.Services.CreateScope();
//    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

//    try
//    {
//        logger.LogInformation("Verificando conexão com banco de dados...");
//        await dbContext.Database.CanConnectAsync();
//        logger.LogInformation("Conectado ao banco de dados com sucesso!");

//        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

//        if (pendingMigrations.Any())
//        {
//            logger.LogWarning($"{pendingMigrations.Count()} migrations pendentes detectadas. Aplicando...");
//            await dbContext.Database.MigrateAsync();
//            logger.LogInformation("Migrations aplicadas com sucesso!");
//        }
//    }
//    catch (Exception ex)
//    {
//        logger.LogError(ex, "Erro ao conectar ou migrar banco de dados");
//        throw;
//    }
//}

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

// Servir arquivos estáticos (storage)
app.UseStaticFiles();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
//app.MapHealthChecks("/health");

// Endpoint de informação
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