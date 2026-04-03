using AutomationPortal.Domain.Abstractions;

namespace AutomationPortal.Domain.Entities;

public sealed class GeminiKey : BaseEntity
{
    public string Name { get; set; } = default!;
    public string KeyValue { get; set; } = default!;
    public Guid UserId { get; set; }

    public GeminiKey() { }

    public static GeminiKey Create(string name, string keyValue, Guid userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > 200)
            throw new ArgumentException("Name must not exceed 200 characters.", nameof(name));

        var trimmedKey = keyValue?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(trimmedKey))
            throw new ArgumentException("KeyValue must not be empty.", nameof(keyValue));
        if (trimmedKey.Length > 500)
            throw new ArgumentException("KeyValue must not exceed 500 characters.", nameof(keyValue));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId must not be empty.", nameof(userId));

        return new GeminiKey
        {
            Name = name,
            KeyValue = trimmedKey,
            UserId = userId,
        };
    }

    public void Update(string name, string keyValue, Guid userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > 200)
            throw new ArgumentException("Name must not exceed 200 characters.", nameof(name));

        var trimmedKey = keyValue?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(trimmedKey))
            throw new ArgumentException("KeyValue must not be empty.", nameof(keyValue));
        if (trimmedKey.Length > 500)
            throw new ArgumentException("KeyValue must not exceed 500 characters.", nameof(keyValue));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId must not be empty.", nameof(userId));

        Name = name;
        KeyValue = trimmedKey;
        UserId = userId;
    }
}
