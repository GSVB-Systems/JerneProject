using System;
using System.Threading.Tasks;
using api.Models;
using dataaccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using service.Repositories;
using service.Repositories.Interfaces;
using service.Services;
using service.Services.Interfaces;
using Sieve.Models;
using Sieve.Services;

namespace api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool addDefaultDbContext = true,
        Action<IServiceCollection>? configureOverrides = null)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        services.Configure<JwtSettings>(options =>
        {
            options.Secret = configuration["JWT_SECRET"]!;
            options.Issuer = configuration["JWT_ISSUER"]!;
            options.Audience = configuration["JWT_AUDIENCE"]!;
        });

        services.AddSingleton<TokenService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = TokenService.ValidationParameters(configuration);
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = _ =>
                    {
                        Console.WriteLine("Token validated successfully");
                        return Task.CompletedTask;
                    }
                };
            });

        if (addDefaultDbContext)
        {
            var connectionString = configuration.GetConnectionString("AppDb") ??
                                    ConnectionStringHelper.BuildPostgresConnectionString();

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        }

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<PasswordService, PasswordService>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<TokenService>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.Configure<SieveOptions>(configuration.GetSection("Sieve"));
        services.AddScoped<ISieveProcessor, SieveProcessor>();
        services.AddScoped<IBoardNumberRepository, BoardNumberRepository>();
        services.AddScoped<IWinningNumberRepository, WinningNumberRepository>();
        services.AddScoped<IWinningboardRepository, WinningBoardRepository>();
        services.AddScoped<IBoardNumberService, BoardNumberService>();
        services.AddScoped<IWinningNumberService, WinningNumberService>();
        services.AddScoped<IWinningBoardService, WinningBoardService>();
        services.AddScoped<IBoardMatcherService, BoardMatchService>();
        services.AddScoped<IPurchaseService, PurchaseService>();

        services.AddAuthorization();
        services.AddCors();
        services.AddControllers();
        services.AddOpenApiDocument();
        services.AddProblemDetails();

        configureOverrides?.Invoke(services);

        return services;
    }
}
