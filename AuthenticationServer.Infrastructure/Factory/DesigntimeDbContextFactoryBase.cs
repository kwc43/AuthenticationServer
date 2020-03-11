using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthenticationServer.Infrastructure.Factory
{
    public abstract class DesigntimeDbContextFactoryBase<TDbContext> : IDesignTimeDbContextFactory<TDbContext> where TDbContext : DbContext
    {
        private readonly string _environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        private readonly string _appDirectory = Directory.GetCurrentDirectory();
        private readonly string _connectionStringName = "Default";
        protected string ConnectionString;

        public TDbContext CreateDbContext(string[] args)
        {
            return Create(_environmentName, _appDirectory, _connectionStringName);
        }

        private TDbContext Create(string environmentName, string appDirectory, string connectionStringName)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(appDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();

            ConnectionString = config.GetConnectionString(connectionStringName);

            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new InvalidOperationException(
                    $"Could not find a connection string named: {connectionStringName}");
            }

            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();

            optionsBuilder.UseSqlServer(ConnectionString);

            return CreateNewInstance(optionsBuilder.Options);
        }

        protected abstract TDbContext CreateNewInstance(DbContextOptions<TDbContext> options);

    }
}
