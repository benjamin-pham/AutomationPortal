using AutomationPortal.Domain.Entities;
using AutomationPortal.Infrastructure.IntegrationTests.Infrastructure;
using AutomationPortal.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutomationPortal.Infrastructure.IntegrationTests.GeminiKeys;

[Collection(nameof(RepositoryTestCollection))]
public sealed class GeminiKeyRepositoryTests(RepositoryTestFixture fixture) : IAsyncLifetime
{
    private readonly RepositoryTestFixture _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddAndGetById_ReturnsGeminiKey()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new GeminiKeyRepository(context);

        var key = GeminiKey.Create("Test Key", "AIzaTestKey1234", Guid.NewGuid());
        repo.Add(key);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(key.Id);

        found.Should().NotBeNull();
        found!.Name.Should().Be("Test Key");
        found.KeyValue.Should().Be("AIzaTestKey1234");
    }

    [Fact]
    public async Task GetByNameAsync_ExistingName_ReturnsGeminiKey()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new GeminiKeyRepository(context);

        var key = GeminiKey.Create("Named Key", "AIzaNamedKey5678", Guid.NewGuid());
        repo.Add(key);
        await context.SaveChangesAsync();

        var found = await repo.GetByNameAsync("Named Key");

        found.Should().NotBeNull();
        found!.Id.Should().Be(key.Id);
    }

    [Fact]
    public async Task GetByNameAsync_NonExistingName_ReturnsNull()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new GeminiKeyRepository(context);

        var found = await repo.GetByNameAsync("Does Not Exist");

        found.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAsync_ExistingUserId_ReturnsGeminiKey()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new GeminiKeyRepository(context);

        var userId = Guid.NewGuid();
        var key = GeminiKey.Create("User Key", "AIzaUserKey9012", userId);
        repo.Add(key);
        await context.SaveChangesAsync();

        var found = await repo.GetByUserIdAsync(userId);

        found.Should().NotBeNull();
        found!.Id.Should().Be(key.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_NonExistingUserId_ReturnsNull()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new GeminiKeyRepository(context);

        var found = await repo.GetByUserIdAsync(Guid.NewGuid());

        found.Should().BeNull();
    }

    [Fact]
    public async Task Add_DuplicateName_ThrowsUniqueConstraintException()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new GeminiKeyRepository(context);

        repo.Add(GeminiKey.Create("Duplicate Name", "AIzaKey1111", Guid.NewGuid()));
        await context.SaveChangesAsync();

        await using var context2 = _fixture.CreateDbContext();
        var repo2 = new GeminiKeyRepository(context2);
        repo2.Add(GeminiKey.Create("Duplicate Name", "AIzaKey2222", Guid.NewGuid()));

        var act = async () => await context2.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Add_DuplicateUserId_ThrowsUniqueConstraintException()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new GeminiKeyRepository(context);

        var userId = Guid.NewGuid();
        repo.Add(GeminiKey.Create("Key A", "AIzaKeyA1234", userId));
        await context.SaveChangesAsync();

        await using var context2 = _fixture.CreateDbContext();
        var repo2 = new GeminiKeyRepository(context2);
        repo2.Add(GeminiKey.Create("Key B", "AIzaKeyB5678", userId));

        var act = async () => await context2.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateException>();
    }
}
