using AutomationPortal.Domain.Entities;

namespace AutomationPortal.Domain.UnitTests.GeminiKeys;

public sealed class GeminiKeyTests
{
    private static readonly Guid ValidUserId = Guid.NewGuid();
    private const string ValidName = "My API Key";
    private const string ValidKeyValue = "AIzaSyAbc123xyz";

    // --- Create() ---

    [Fact]
    public void Create_WithValidArguments_ReturnsGeminiKey()
    {
        var key = GeminiKey.Create(ValidName, ValidKeyValue, ValidUserId);

        key.Name.Should().Be(ValidName);
        key.KeyValue.Should().Be(ValidKeyValue);
        key.UserId.Should().Be(ValidUserId);
        key.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_EachCallProducesDifferentId()
    {
        var key1 = GeminiKey.Create("Key 1", ValidKeyValue, ValidUserId);
        var key2 = GeminiKey.Create("Key 2", ValidKeyValue, Guid.NewGuid());

        key1.Id.Should().NotBe(key2.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ThrowsArgumentException(string name)
    {
        var act = () => GeminiKey.Create(name, ValidKeyValue, ValidUserId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNameExceedingMaxLength_ThrowsArgumentException()
    {
        var longName = new string('a', 201);

        var act = () => GeminiKey.Create(longName, ValidKeyValue, ValidUserId);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyKeyValue_ThrowsArgumentException(string keyValue)
    {
        var act = () => GeminiKey.Create(ValidName, keyValue, ValidUserId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithKeyValueExceedingMaxLength_ThrowsArgumentException()
    {
        var longKey = new string('k', 501);

        var act = () => GeminiKey.Create(ValidName, longKey, ValidUserId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsArgumentException()
    {
        var act = () => GeminiKey.Create(ValidName, ValidKeyValue, Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_TrimsKeyValueWhitespace()
    {
        var key = GeminiKey.Create(ValidName, "  AIzaSyAbc123  ", ValidUserId);

        key.KeyValue.Should().Be("AIzaSyAbc123");
    }

    // --- Update() ---

    [Fact]
    public void Update_WithValidArguments_UpdatesProperties()
    {
        var key = GeminiKey.Create(ValidName, ValidKeyValue, ValidUserId);
        var newUserId = Guid.NewGuid();

        key.Update("Updated Name", "NewKeyValue", newUserId);

        key.Name.Should().Be("Updated Name");
        key.KeyValue.Should().Be("NewKeyValue");
        key.UserId.Should().Be(newUserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithEmptyName_ThrowsArgumentException(string name)
    {
        var key = GeminiKey.Create(ValidName, ValidKeyValue, ValidUserId);

        var act = () => key.Update(name, ValidKeyValue, ValidUserId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_WithNameExceedingMaxLength_ThrowsArgumentException()
    {
        var key = GeminiKey.Create(ValidName, ValidKeyValue, ValidUserId);
        var longName = new string('a', 201);

        var act = () => key.Update(longName, ValidKeyValue, ValidUserId);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithEmptyKeyValue_ThrowsArgumentException(string keyValue)
    {
        var key = GeminiKey.Create(ValidName, ValidKeyValue, ValidUserId);

        var act = () => key.Update(ValidName, keyValue, ValidUserId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_WithKeyValueExceedingMaxLength_ThrowsArgumentException()
    {
        var key = GeminiKey.Create(ValidName, ValidKeyValue, ValidUserId);
        var longKey = new string('k', 501);

        var act = () => key.Update(ValidName, longKey, ValidUserId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_WithEmptyUserId_ThrowsArgumentException()
    {
        var key = GeminiKey.Create(ValidName, ValidKeyValue, ValidUserId);

        var act = () => key.Update(ValidName, ValidKeyValue, Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_TrimsKeyValueWhitespace()
    {
        var key = GeminiKey.Create(ValidName, ValidKeyValue, ValidUserId);

        key.Update(ValidName, "  TrimmedKey  ", ValidUserId);

        key.KeyValue.Should().Be("TrimmedKey");
    }
}
