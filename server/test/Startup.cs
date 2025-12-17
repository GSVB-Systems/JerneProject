using api.Extensions;
using dataaccess;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace test;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        Env.Load();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddApiServices(configuration, addDefaultDbContext: false, overrides =>
        {
            overrides.RemoveAll(typeof(AppDbContext));

            overrides.AddScoped<AppDbContext>(_ =>
            {
                var postgreSqlContainer = new PostgreSqlBuilder().Build();
                postgreSqlContainer.StartAsync().GetAwaiter().GetResult();
                var connectionString = postgreSqlContainer.GetConnectionString();
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseNpgsql(connectionString)
                    .Options;

                var ctx = new AppDbContext(options);
                ctx.Database.EnsureCreated();
                return ctx;
            });
        });
    }
}