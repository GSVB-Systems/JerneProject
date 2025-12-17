using DotNetEnv;
using api;
using api.Extensions;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.GenerateApiClientsFromOpenApi("/../../client/src/models/ServerAPI.ts").GetAwaiter().GetResult();

app.UseCors(config => config
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
    .SetIsOriginAllowed(_ => true)
);

app.UseHttpsRedirection();

app.UseOpenApi();
app.UseSwaggerUi();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
