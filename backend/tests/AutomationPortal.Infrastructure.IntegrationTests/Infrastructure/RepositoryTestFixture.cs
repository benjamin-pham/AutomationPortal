using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Infrastructure.Data;

namespace AutomationPortal.Infrastructure.IntegrationTests.Infrastructure;

public sealed class RepositoryTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .Build();

    private Respawner _respawner = null!;
    private NpgsqlConnection _dbConnection = null!;

    public string ConnectionString => _dbContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var context = CreateDbContext();
        await context.Database.MigrateAsync();

        _dbConnection = new NpgsqlConnection(ConnectionString);
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter        = DbAdapter.Postgres,
                SchemasToInclude = ["public"]
            });
    }

    public async Task ResetDatabaseAsync() =>
        await _respawner.ResetAsync(_dbConnection);

    public AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new AppDbContext(options, new TestDateTimeProvider(), new TestUserContext());
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbConnection.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }

    private sealed class TestDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }

    private sealed class TestUserContext : IUserContext
    {
        public Guid UserId => Guid.Empty;
    }
}

[CollectionDefinition(nameof(RepositoryTestCollection))]
public sealed class RepositoryTestCollection : ICollectionFixture<RepositoryTestFixture>;
