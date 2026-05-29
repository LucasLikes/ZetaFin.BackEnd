using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ZetaFin.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Caminho relativo — funciona em qualquer máquina
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ZetaFin.API");
        var configPath = Path.Combine(basePath, "appsettings.json");

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"❌ Não foi possível localizar o arquivo de configuração em: {configPath}");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}