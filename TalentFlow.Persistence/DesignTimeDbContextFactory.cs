using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TalentFlow.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TalentFlowDbContext>
    {
        public TalentFlowDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<TalentFlowDbContext>();

            // ✅ ALWAYS USE POSTGRESQL
            optionsBuilder.UseNpgsql(connectionString);

            return new TalentFlowDbContext(optionsBuilder.Options);
        }
    }
}