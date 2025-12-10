using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using api;
using api.Models;
using dataaccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NSwag;
using service.Services;
using service.Repositories;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;
using Sieve.Services;

var builder = WebApplication.CreateBuilder(args);



Env.Load();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        // Globally require users to be authenticated
        .RequireAuthenticatedUser()
        .Build();
});

var connectionString = builder.Configuration.GetConnectionString("AppDb") ?? ConnectionStringHelper.BuildPostgresConnectionString();
builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<JwtSettings>(options =>
{
    options.Secret = builder.Configuration["JWT_SECRET"]!;
    options.Issuer = builder.Configuration["JWT_ISSUER"]!;
    options.Audience = builder.Configuration["JWT_AUDIENCE"]!;
});
builder.Services.AddSingleton<TokenService>();

var secret = builder.Configuration["JWT_SECRET"]!;
var key = Encoding.UTF8.GetBytes(secret);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        

        options.TokenValidationParameters = TokenService.ValidationParameters(
            builder.Configuration
        );
        // Add this for debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);


builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBoardRepository, BoardRepository>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<service.Services.PasswordService, PasswordService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("Sieve"));
builder.Services.AddScoped<ISieveProcessor, SieveProcessor>();
builder.Services.AddScoped<IBoardNumberRepository, BoardNumberRepository>();
builder.Services.AddScoped<IWinningNumberRepository, WinningNumberRepository>();
builder.Services.AddScoped<IBoardNumberService, BoardNumberService>();
builder.Services.AddScoped<IWinningNumberService, WinningNumberService>();


builder.Services.AddAuthorization();
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.GenerateApiClientsFromOpenApi("/../../client/src/models/ServerAPI.ts").GetAwaiter().GetResult();


app.UseCors(config => config
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
    .SetIsOriginAllowed(x => true)
);

app.UseHttpsRedirection();

app.UseOpenApi();
app.UseSwaggerUi();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();