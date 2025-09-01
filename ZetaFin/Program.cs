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

// Configurar a autentica��o JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  // Remover CookieAuthenticationDefaults, j� que voc� est� usando JWT
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,    // Caso precise configurar o Issuer, fa�a aqui
            ValidateAudience = false,  // Caso precise configurar o Audience, fa�a aqui
            ValidateLifetime = true,   // Verifica se o token expirou
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))  // A chave secreta configurada em appsettings.json
        };
    });

// Adicionando o servi�o de autoriza��o
builder.Services.AddAuthorization();

// Injec��o de depend�ncias para servi�os
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddScoped<IDepositRepository, DepositRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserGoalService, UserGoalService>();

// Configura��o para enviar e-mails (caso seja necess�rio adicionar o servi�o de email depois)
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
app.UseAuthentication(); // Habilita autentica��o JWT
app.UseAuthorization();  // Habilita autoriza��o

app.MapControllers();

app.Run();
