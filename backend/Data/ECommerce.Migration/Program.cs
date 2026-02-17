using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ECommerce.Migration;
using System.CommandLine;

AppDomain.CurrentDomain.UnhandledException += (_, _) => Environment.Exit(1);

var upOption = new Option<bool>("--up")
{
    DefaultValueFactory = (b) => false,
    Description = "Migrate Up"
};
var downOption = new Option<long>("--down")
{
    DefaultValueFactory = (l) => -1,
    Description = "Rollback database to a version",
};
var envOption = new Option<string>("--env")
{
    DefaultValueFactory = (s) => "Default",
    Description = "Set Environment",
};

var rootCommand = new RootCommand("ECommerce Fluent Migrator Runner")
{
    upOption,
    downOption,
    envOption
};

rootCommand.SetAction(parseResult =>
{
    var up = parseResult.GetValue(upOption);
    var down = parseResult.GetValue(downOption);
    var env = parseResult.GetValue(envOption);

    var serviceProvider = CreateServices(env!);

    using (var scope = serviceProvider.CreateScope())
    {
        if (up)
            UpdateDatabase(scope.ServiceProvider);

        if (down > -1)
            RollbackDatabase(scope.ServiceProvider, down);
    }
});

var result = rootCommand.Parse(args);
await result.InvokeAsync();

static IServiceProvider CreateServices(string env)
{
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

    var conn = config[$"connectionString:{env}"];

    return new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb =>
            rb
            .AddPostgres()
                .WithGlobalConnectionString(conn)
                .ScanIn(typeof(Tables).Assembly)
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

static void RollbackDatabase(IServiceProvider serviceProvider, long rollbackVersion)
{
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateDown(rollbackVersion);
}
