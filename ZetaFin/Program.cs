using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ZetaFin.Application.Interfaces;
using ZetaFin.Application.Services;
using ZetaFin.Domain.Interfaces;
using ZetaFin.Persistence.Repositories;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar EF Core (use seu connection string real)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar a autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  // Remover CookieAuthenticationDefaults, já que você está usando JWT
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,    // Caso precise configurar o Issuer, faça aqui
            ValidateAudience = false,  // Caso precise configurar o Audience, faça aqui
            ValidateLifetime = true,   // Verifica se o token expirou
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))  // A chave secreta configurada em appsettings.json
        };
    });

// Adicionando o serviço de autorização
builder.Services.AddAuthorization();

// Injecção de dependências para serviços
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddScoped<IDepositRepository, DepositRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserGoalService, UserGoalService>();

// Configuração para enviar e-mails (caso seja necessário adicionar o serviço de email depois)
// builder.Services.AddScoped<IEmailService, EmailService>();
// builder.Services.AddSingleton<IEmailService, EmailService>();

var app = builder.Build();

// Enable Swagger in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable authentication and authorization middleware
app.UseAuthentication(); // Habilita autenticação JWT
app.UseAuthorization();  // Habilita autorização

app.MapControllers();

app.Run();
