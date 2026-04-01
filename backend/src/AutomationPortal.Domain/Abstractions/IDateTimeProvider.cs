namespace AutomationPortal.Domain.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
