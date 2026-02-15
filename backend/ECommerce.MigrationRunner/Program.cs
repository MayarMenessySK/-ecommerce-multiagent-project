using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.MigrationRunner;

class Program
{
    static int Main(string[] args)
    {
        bool up = args.Contains("--up");
        long down = -1;
        string env = "Development";

        // Parse arguments
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--down" && i + 1 < args.Length)
                long.TryParse(args[i + 1], out down);
            
            if (args[i] == "--env" && i + 1 < args.Length)
                env = args[i + 1];
        }

        var serviceProvider = CreateServices(env);

        using var scope = serviceProvider.CreateScope();
        
        if (up)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n🔄 Running migrations UP...\n");
            Console.ResetColor();
            
            UpdateDatabase(scope.ServiceProvider);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✅ Migrations completed successfully!\n");
            Console.ResetColor();
            return 0;
        }

        if (down > -1)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n⚠️  Rolling back to version {down}...\n");
            Console.ResetColor();
            
            RollbackDatabase(scope.ServiceProvider, down);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✅ Rollback completed!\n");
            Console.ResetColor();
            return 0;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run -- --up                  # Migrate to latest");
        Console.WriteLine("  dotnet run -- --down 1              # Rollback to version 1");
        Console.WriteLine("  dotnet run -- --up --env Production # Use production connection");
        Console.ResetColor();
        return 1;
    }

    static IServiceProvider CreateServices(string env)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = config[$"ConnectionStrings:{env}"];

        if (string.IsNullOrEmpty(connectionString))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Connection string not found for environment: {env}");
            Console.ResetColor();
            Environment.Exit(1);
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"📊 Environment: {env}");
        Console.WriteLine($"🔌 Database: {GetDatabaseName(connectionString)}\n");
        Console.ResetColor();

        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(ECommerce.Data.Migrations.V1_InitialSchema).Assembly)
                .For.Migrations()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }

    static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    static void RollbackDatabase(IServiceProvider serviceProvider, long version)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(version);
    }

    static string GetDatabaseName(string connectionString)
    {
        var parts = connectionString.Split(';');
        var dbPart = parts.FirstOrDefault(p => p.Trim().StartsWith("Database=", StringComparison.OrdinalIgnoreCase));
        return dbPart?.Split('=').Last() ?? "unknown";
    }
}
