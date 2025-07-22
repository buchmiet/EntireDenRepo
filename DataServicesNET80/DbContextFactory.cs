using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataServicesNET80;

public static class DbContextFactory
{
    private static ILoggerFactory _loggerFactory;
    private static ILogger _logger;

    public static void ConfigureLoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger("DbContextFactory");
    }

    private static string _connectionString;

    public static void SetDatabaseConfiguration(string host, string port, string user, string password, string database)
    {
        _connectionString = $"server={host};port={port};user={user};password={password};TreatTinyAsBoolean=true;database={database}";
        _logger?.LogInformation("Database configuration set: Host={Host}, Port={Port}, User={User}, Database={Database}",
            host, port, user, database);
    }

    private static DbContextOptions<Time4PartsContext> CreateOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<Time4PartsContext>();
        if (_loggerFactory != null)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
        optionsBuilder.UseMySql(_connectionString, new MySqlServerVersion(new Version(11, 3)));
        return optionsBuilder.Options;
    }

    public static Time4PartsContext GetContext() => new(CreateOptions());

    public static Time4PartsContext GetWriteableContext() => new(CreateOptions());
}