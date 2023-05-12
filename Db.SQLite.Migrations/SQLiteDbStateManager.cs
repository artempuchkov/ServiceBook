using System.Reflection;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;

namespace Db.SQLite.Migrations;

public class SQLiteDbStateManager
{
	private ServiceProvider _serviceProvider;
	private bool _initialized;
	private readonly string _connectionString;

	public IVersionTableMetaData VersionTable => new VersionTable();

	public SQLiteDbStateManager(string connectionString)
	{
		_connectionString = connectionString;
		_connectionString.EnsureSQLiteDbPath();
	}

	public void UpdateDatabase(long? version = null)
	{
		EnsureInitialization();

		using (var scope = _serviceProvider.CreateScope())
		{
			var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
			if (version != null)
			{
				runner.MigrateUp(version.Value);
			}
			else
			{
				runner.MigrateUp();
			}
		}
	}

	private void EnsureInitialization()
	{
		if (!_initialized)
		{
			_serviceProvider = CreateServices();
			_initialized = true;
		}
	}

	private ServiceProvider CreateServices(bool generateScriptsOnly = false)
	{
		var services = new ServiceCollection();
		services.AddFluentMigratorCore().ConfigureRunner(rb =>
			rb.AddConcreteDbRunner(this).
			WithVersionTable(this).
			WithGlobalConnectionString(_connectionString).
			ScanIn(MigrationsAssemblies()).For.Migrations()
		).AddLogging(lb => lb.AddFluentMigratorConsole());

		services.Configure<ProcessorOptions>(opt =>
		{
			opt.PreviewOnly = generateScriptsOnly;
			opt.Timeout = TimeSpan.FromSeconds(600);
		});

		return services.BuildServiceProvider(false);
	}

	protected internal IMigrationRunnerBuilder AddConcreteDbRunner(IMigrationRunnerBuilder builder)
	{
		return builder.AddSQLite();
	}

	private Assembly[] MigrationsAssemblies()
	{
		return new[] { typeof(PreInitial).Assembly };
	}

	public void Dispose()
	{
		_serviceProvider?.Dispose();
	}
}

internal static class DbStateManagerBaseExtensions
{
	public static IMigrationRunnerBuilder AddConcreteDbRunner(this IMigrationRunnerBuilder builder, SQLiteDbStateManager runner)
	{
		return runner.AddConcreteDbRunner(builder);
	}

	public static IMigrationRunnerBuilder WithVersionTable(this IMigrationRunnerBuilder builder, SQLiteDbStateManager runner)
	{
		return runner.VersionTable != null ? builder.WithVersionTable(runner.VersionTable) : builder;
	}
}