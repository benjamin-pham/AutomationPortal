using AutomationPortal.Domain.Abstractions;

namespace AutomationPortal.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
