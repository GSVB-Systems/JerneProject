using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using api.Models;
using dataaccess;
using Microsoft.EntityFrameworkCore;
using service.Models;
using service.Services;
using Service.Repositories;
var builder = WebApplication.CreateBuilder(args);



Env.Load();
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
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT_ISSUER"],
            ValidAudience = builder.Configuration["JWT_AUDIENCE"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
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
builder.Services.AddScoped<service.Services.PasswordService>();

builder.Services.AddAuthorization();
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();
builder.Services.AddProblemDetails();


var app = builder.Build();

app.UseCors(config => config
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
    .SetIsOriginAllowed(x => true)
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseOpenApi();
app.UseSwaggerUi();

app.Run();