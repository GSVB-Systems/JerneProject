namespace dataaccess;

public static class ConnectionStringHelper
{
    public static string BuildPostgresConnectionString()
    {
        string Get(string key) =>
            Environment.GetEnvironmentVariable(key)
            ?? throw new Exception($"Missing environment variable: {key}");

        var host = Get("POSTGRES_HOST");
        var database = Get("POSTGRES_DB");
        var user = Get("POSTGRES_USER");
        var password = Get("POSTGRES_PASSWORD");
        var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";

        return $"Host={host};Port={port};Database={database};Username={user};Password={password};SslMode=Require";
    }
}