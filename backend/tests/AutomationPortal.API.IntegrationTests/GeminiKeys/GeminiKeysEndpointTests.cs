using AutomationPortal.API.IntegrationTests.Infrastructure;
using AutomationPortal.Domain.Entities;
using AutomationPortal.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace AutomationPortal.API.IntegrationTests.GeminiKeys;

public sealed class GeminiKeysEndpointTests(CustomWebApplicationFactory factory)
    : BaseIntegrationTest(factory)
{
    private const string Endpoint = "/api/gemini-keys";

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<Guid> SeedUserAsync(string username = "testuser")
    {
        var response = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "Test",
            lastName  = "User",
            username,
            password  = "Secret123"
        });
        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        return body!.UserId;
    }

    private async Task SeedGeminiKeyAsync(string name, string keyValue, Guid userId)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var key = GeminiKey.Create(name, keyValue, userId);
        db.GeminiKeys.Add(key);
        await db.SaveChangesAsync();
    }

    // ── GET /api/gemini-keys ─────────────────────────────────────────────────

    [Fact]
    public async Task GetGeminiKeys_WithoutAuth_Returns401()
    {
        var response = await Client.GetAsync(Endpoint);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetGeminiKeys_EmptyDatabase_Returns200WithEmptyPage()
    {
        var userId = await SeedUserAsync("getkeys_emptyuser");
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(userId));

        var response = await Client.GetAsync($"{Endpoint}?pageNumber=1&pageSize=20");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PagedResponse<GeminiKeyListItemDto>>();
        body!.Items.Should().BeEmpty();
        body.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task GetGeminiKeys_WithData_Returns200WithPagedResponse()
    {
        var userId = await SeedUserAsync("getkeys_datauser");
        await SeedGeminiKeyAsync("My Key", "AIzaSyTestKey1234567890", userId);

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(userId));

        var response = await Client.GetAsync($"{Endpoint}?pageNumber=1&pageSize=20");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PagedResponse<GeminiKeyListItemDto>>();
        body!.Items.Should().HaveCount(1);
        body.TotalItems.Should().Be(1);

        var item = body.Items[0];
        item.Name.Should().Be("My Key");
        item.AssignedUsername.Should().Be("getkeys_datauser");
    }

    [Fact]
    public async Task GetGeminiKeys_MaskedKey_FormattedCorrectly()
    {
        var userId = await SeedUserAsync("getkeys_maskuser");
        await SeedGeminiKeyAsync("Mask Test Key", "AIzaSyAbcDefGhij", userId);

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(userId));

        var response = await Client.GetAsync($"{Endpoint}?pageNumber=1&pageSize=20");
        var body = await response.Content.ReadFromJsonAsync<PagedResponse<GeminiKeyListItemDto>>();

        var item = body!.Items[0];
        item.MaskedKey.Should().StartWith("****");
        item.MaskedKey.Should().Be("****Ghij");
    }

    [Fact]
    public async Task GetGeminiKeys_ResponseDoesNotContainRawKeyValue()
    {
        var userId = await SeedUserAsync("getkeys_rawuser");
        await SeedGeminiKeyAsync("Raw Key Test", "AIzaSySecretValue1234", userId);

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(userId));

        var responseText = await Client.GetStringAsync($"{Endpoint}?pageNumber=1&pageSize=20");

        responseText.Should().NotContain("AIzaSySecretValue1234");
        responseText.Should().NotContain("keyValue");
        responseText.Should().NotContain("key_value");
    }

    // ── POST /api/gemini-keys ────────────────────────────────────────────────

    [Fact]
    public async Task CreateGeminiKey_Returns201_WhenRequestIsValid()
    {
        var userId = await SeedUserAsync("postkey_user");
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(userId));

        var response = await Client.PostAsJsonAsync(Endpoint, new
        {
            name = "New Key",
            keyValue = "AIzaSyABCDEF12345678",
            userId = userId,
            replaceExisting = false
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateGeminiKey_Returns409_WhenUserAlreadyHasKey_AndNotReplacing()
    {
        var userId = await SeedUserAsync("postkey_409_user");
        await SeedGeminiKeyAsync("Old Key", "AIzaOldValue", userId);

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(userId));

        var response = await Client.PostAsJsonAsync(Endpoint, new
        {
            name = "New Key",
            keyValue = "AIzaNewValue",
            userId = userId,
            replaceExisting = false
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateGeminiKey_Returns201_WhenUserAlreadyHasKey_AndReplacing()
    {
        var userId = await SeedUserAsync("postkey_replace_user");
        await SeedGeminiKeyAsync("Old Key", "AIzaOldValue", userId);

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(userId));

        var response = await Client.PostAsJsonAsync(Endpoint, new
        {
            name = "New Key",
            keyValue = "AIzaNewValue",
            userId = userId,
            replaceExisting = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // ── PUT /api/gemini-keys/{id} ───────────────────────────────────────────

    [Fact]
    public async Task UpdateGeminiKey_Returns204_WhenRequestIsValid()
    {
        var userId = await SeedUserAsync("putkey_user");
        var keyName = "Original Key";
        await SeedGeminiKeyAsync(keyName, "AIzaSyOriginalKey", userId);

        var existingKey = await GetExistingKeyAsync(keyName);

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(userId));

        var response = await Client.PutAsJsonAsync($"{Endpoint}/{existingKey.Id}", new
        {
            nameOfNewKey = "Updated Name",
            keyValue = "AIzaSyNewKey",
            userId = userId,
            replaceExisting = false
        });

        // Wait, the property name should be 'name' not 'nameOfNewKey'
        var payload = new
        {
            name = "Updated Name",
            keyValue = "AIzaSyNewKey",
            userId = userId,
            replaceExisting = false
        };

        response = await Client.PutAsJsonAsync($"{Endpoint}/{existingKey.Id}", payload);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updated = await GetExistingKeyAsync("Updated Name");
        updated.Id.Should().Be(existingKey.Id);
        updated.KeyValue.Should().Be("AIzaSyNewKey");
    }

    [Fact]
    public async Task UpdateGeminiKey_Returns422_WhenNameAlreadyExistsOnOtherRecord()
    {
        var user1 = await SeedUserAsync("putkey_name_u1");
        var user2 = await SeedUserAsync("putkey_name_u2");
        await SeedGeminiKeyAsync("Key One", "AIzaKeyOne", user1);
        await SeedGeminiKeyAsync("Key Two", "AIzaKeyTwo", user2);

        var keyOne = await GetExistingKeyAsync("Key One");

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(user1));

        var response = await Client.PutAsJsonAsync($"{Endpoint}/{keyOne.Id}", new
        {
            name = "Key Two",
            keyValue = "AIzaKeyOneNewValue",
            userId = user1,
            replaceExisting = false
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UpdateGeminiKey_Returns409_WhenUserAlreadyHasOtherKey_AndNotReplacing()
    {
        var user1 = await SeedUserAsync("putkey_conflict_u1");
        var user2 = await SeedUserAsync("putkey_conflict_u2");
        await SeedGeminiKeyAsync("User One Key", "AIzaK1", user1);
        await SeedGeminiKeyAsync("User Two Key", "AIzaK2", user2);

        var keyOne = await GetExistingKeyAsync("User One Key");

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(user1));

        var response = await Client.PutAsJsonAsync($"{Endpoint}/{keyOne.Id}", new
        {
            name = "User One Key",
            keyValue = "AIzaK1",
            userId = user2, // Change owner to user2 who already has a key
            replaceExisting = false
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateGeminiKey_KeepsOriginalValue_WhenMaskedKeySent()
    {
        var userId = await SeedUserAsync("putkey_mask_user");
        var originalValue = "AIzaSySecretOriginal";
        await SeedGeminiKeyAsync("Mask Test", originalValue, userId);

        var existingKey = await GetExistingKeyAsync("Mask Test");

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(userId));

        var response = await Client.PutAsJsonAsync($"{Endpoint}/{existingKey.Id}", new
        {
            name = "Updated Name",
            keyValue = "****inal", // Masked value
            userId = userId,
            replaceExisting = false
        });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updated = await GetExistingKeyAsync("Updated Name");
        updated.KeyValue.Should().Be(originalValue);
    }

    // ── DELETE /api/gemini-keys/{id} ─────────────────────────────────────────

    [Fact]
    public async Task DeleteGeminiKey_Returns204_WhenKeyExists()
    {
        var userId = await SeedUserAsync("delkey_user");
        var name = "Delete Me";
        await SeedGeminiKeyAsync(name, "AIzaSyDeleteKey", userId);

        var existingKey = await GetExistingKeyAsync(name);

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(userId));

        var response = await Client.DeleteAsync($"{Endpoint}/{existingKey.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var deleted = await db.GeminiKeys.IgnoreQueryFilters()
            .FirstOrDefaultAsync(k => k.Id == existingKey.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteGeminiKey_Returns404_WhenKeyDoesNotExist()
    {
        var userId = await SeedUserAsync("delkey_404_user");
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(userId));

        var response = await Client.DeleteAsync($"{Endpoint}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<GeminiKey> GetExistingKeyAsync(string name)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.GeminiKeys.FirstAsync(k => k.Name == name);
    }

    // ── DTOs ─────────────────────────────────────────────────────────────────

    private sealed record RegisterResponse(Guid UserId, string Username, string FirstName, string LastName);

    private sealed record PagedResponse<T>(
        List<T> Items,
        int PageNumber,
        int PageSize,
        int TotalItems,
        int TotalPages);

    private sealed record GeminiKeyListItemDto(
        Guid Id,
        string Name,
        string MaskedKey,
        Guid AssignedUserId,
        string AssignedUsername,
        DateTime CreatedAt);
}
