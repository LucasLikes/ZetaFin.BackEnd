using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ZetaFin.Persistence;
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Caminho absoluto para o appsettings.json da API
            var configPath = @"C:\Users\lucas\source\repos\ZetaFin.BackEnd\ZetaFin.API\appsettings.json";

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"❌ Não foi possível localizar o arquivo de configuração em: {configPath}");
            }

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(configPath, optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));


            return new ApplicationDbContext(optionsBuilder.Options);
        }

    }