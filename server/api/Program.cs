using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using api.Models;
using api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
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